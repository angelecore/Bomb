using bomberman.classes.facade;
using bomberman.classes.memento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{

    public class GameState
    {
        public string PlayerName { get; set; }
        public string? PlayerId { get; set; }

        public Block[,] Grid;

        public List<Fire> FireList = new List<Fire>();

        public List<FireController> FireControllerList = new List<FireController>();

        public int[,] ExplosionIntensity;

        private List<Vector2f> possiblePlayerPos = new List<Vector2f>();

        private List<Player> players = new List<Player>();

        public List<Bomb> Bombs = new List<Bomb>();

        public List<Tuple<object, Player>> scoreEvents = new List<Tuple<object, Player>>();

        // key is the index of the Grid tile
        public Dictionary<int, IPowerup> Powerups = new Dictionary<int, IPowerup>();
        public Dictionary<int, IBombtype> Bombtypes = new Dictionary<int, IBombtype>();

        private ConcreteObserver form;

        // key is the player id
        // each player will have a seperate player snapshot manager
        public Dictionary<string, PlayerSnapshotManager> PlayerSnapshots = new Dictionary<string, PlayerSnapshotManager>();

        private float secondTimer = 1.0f;
        public void SetForm(ConcreteObserver form)
        {
            this.form = form;
        }

        public GameState(string playerName, int maxPlayerCount = 2)
        {
            this.PlayerName = playerName;
            GameDataSingleton.GetInstance().SetCurrentGameStatus(GameStatus.WaitingForPlayers);
            LoadMap();
            GameDataSingleton.GetInstance().SetMaxPlayerCount(maxPlayerCount);
        }

        public int GetGridIndex(Vector2f position)
        {
            return position.Y * GameDataSingleton.GetInstance().Width + position.X;
        }

        public void ActivateReverseMode(Player initiator)
        {
            var affectedPlayer = players.SingleOrDefault(p => p.Id != initiator.Id);
            if (affectedPlayer == null)
            {
                return;
            }

            PlayerSnapshot ?lastSnapshot = null;
            for (int i = 0; i < 5; i++)
            {
                var snapshot = PlayerSnapshots[affectedPlayer.Id].PopLastSnapshot();
                if (snapshot != null)
                {
                    lastSnapshot = snapshot;
                }
            }

            if (lastSnapshot != null)
            {
                affectedPlayer.ApplySnapshot(lastSnapshot);
                form.SetPlayerPosition(affectedPlayer.Id, affectedPlayer.Position);
            }
        }

        private void RemoveBox(Player responsiblePlayer, Vector2f position)
        {
            var cell = Grid[position.Y, position.X];
            //if(responsiblePlayer.BombType == BombType.Fire)
                //cell.Type = BlockType.Fire;
            //else
            bool flag = false;
            if(cell.Type != BlockType.Empty)
            {
                scoreEvents.Add(new Tuple<object, Player>(Constants.SCORE_STRATEGY_DESTROYED_BLOCK, responsiblePlayer));
                flag = true;
            }
            cell.Type = BlockType.Empty;

            int gridIndex = GetGridIndex(position);
            //var temp = new List<BombType> { BombType.Fire, BombType.Fire, BombType.Fire };
            // add-on powerup logic!
            //var random = new Random();
            if (flag)
            {
                if (responsiblePlayer.Score % 15 == 0)
                {
                    Bombtypes.Add(gridIndex, new ChangeBombTypeStrategy(BombType.Basic));
                    return;
                }

                
                if (responsiblePlayer.Score % 10 == 0)
                {
                    //Bombtypes.Add(gridIndex, new ChangeBombTypeStrategy(temp[random.Next(0, 2)]));
                    Bombtypes.Add(gridIndex, new ChangeBombTypeStrategy(BombType.Cluster));
                    return;
                }

                if (responsiblePlayer.Score % 8 == 0)
                {
                    Bombtypes.Add(gridIndex, new ChangeBombTypeStrategy(BombType.Fire));
                    return;
                }

                // Every Sixth crate is a bomb change
                if (responsiblePlayer.Score % 6 == 0)
                {
                    Bombtypes.Add(gridIndex, new ChangeBombTypeStrategy(BombType.Dynamite));
                    return;
                }

                IPowerup ?newPowerup = PowerupFactory.GetPowerupInstance(responsiblePlayer.Score);
                if (newPowerup != null)
                {
                    Powerups.Add(gridIndex, newPowerup);
                }
            }
            //return;
        }

        public void CheckGameStatus()
        {
            var currentGameStatus = GameDataSingleton.GetInstance().CurrentGameStatus;
            if (currentGameStatus == GameStatus.WaitingForPlayers)
            {
                if (players.Count == GameDataSingleton.GetInstance().MaxPlayerCount)
                {
                    GameDataSingleton.GetInstance().SetCurrentGameStatus(GameStatus.InProgress);
                }
            }
            else
            {
                if (players.Count == 0)
                {
                    GameDataSingleton.GetInstance().SetCurrentGameStatus(GameStatus.Tie);
                }

                if (players.Count == 1)
                {
                    GameDataSingleton.GetInstance().SetCurrentGameStatus(players.First().Id == PlayerId ? GameStatus.Won : GameStatus.Lost);
                }
            }

        }
       
        public void RemovePlayer(string playerId)
        {
            players.RemoveAll(p => p.Id == playerId);
        }

        public List<Player> GetKilledPlayers()
        {
            return players
                .Where(p => !p.IsAlive)
                .ToList();
        }

        public void UpdateTick(float miliSecondsPassed)
        {
            bool takeSnapshot = false;
            secondTimer -= miliSecondsPassed * 0.001f;
            if (secondTimer <= 0)
            {
                takeSnapshot = true;
                secondTimer = 1.0f;
            }

            foreach (var player in players)
            {
                player.UpdateTemporaryStats(miliSecondsPassed);

                if (takeSnapshot)
                {
                    if (!PlayerSnapshots.ContainsKey(player.Id))
                    {
                        PlayerSnapshots.Add(player.Id, new PlayerSnapshotManager());
                    }

                    PlayerSnapshots[player.Id].AddSnapshot(player.SnapshotPlayerInfo());
                }
            }


        }

        public void RemoveExplodedTiles(List<Tuple<Vector2f, int>> cells, Player owner, FireController fire)
        {
            foreach(var cell in cells)
            {
                var pos = cell.Item1;
                ExplosionIntensity[pos.Y, pos.X] = cell.Item2;

                // If another bomb is also is this direction, then also explode this bomb next tic.
                foreach (var anotherBomb in Bombs)
                {
                    if (anotherBomb.Position.Equals(pos))
                    {
                        anotherBomb.Timer = 0;
                    }
                }

                foreach (var player in players)
                {
                    if (player.Position.Equals(pos))
                    {
                        player.IsAlive = false;
                    }
                }

                var tyle = Grid[pos.Y, pos.X];
                if (owner.BombType == BombType.Fire)
                {
                    FireController clone = (FireController) fire.Clone();
                    clone.BlockX = pos.X;
                    clone.BlockY = pos.Y;
                    FireControllerList.Add(clone);
                    tyle.Type = BlockType.Fire;
                }
                
                 // Destroy this block
                 RemoveBox(owner, pos);
                
            }
        }

        public List<Bomb> UpdateBombTimers(float miliSeconds)
        {
            var explodedBombs = new List<Bomb>();
            for(int i = Bombs.Count - 1; i >= 0; i--)
            {
                var bomb = Bombs[i];
                // 1 tick
                bomb.Timer -= (float)miliSeconds * 0.001f;
                if (bomb.Timer < 1)
                {
                    Fire fire = null;
                    var cells = bomb.GetExplosionPositions(Grid, (pos) => IsPositionValid(pos));
                    FireController controller = null;
                    if (bomb.Owner.BombType == BombType.Fire)
                    {
                        fire = new Fire(DateTime.Now.Millisecond.ToString());
                        controller = new FireController(fire.ID,fire,bomb.Position.X,bomb.Position.Y);
                        FireList.Add(fire);
                    }
                    Bombs.RemoveAt(i);
                    Console.WriteLine(cells.GetType());
                    RemoveExplodedTiles(cells, bomb.Owner, controller);
                    explodedBombs.Add(bomb);
                    if (bomb.Owner.BombType == BombType.Cluster && bomb.Generation < 2)
                    {
                        foreach (var cell in cells)
                        {
                            bool flag = true;
                            if (cell.Item1.Equals(bomb.Position))
                                continue;
                            Bomb clone = (Bomb) bomb.Clone(cell.Item1, bomb.Generation+1);
                            foreach (var otherBomb in Bombs)
                            {
                                if (otherBomb.Position.Equals(clone.Position))
                                {
                                        flag=false;
                                }
                            }
                            if (flag)
                            {
                                Bombs.Add(clone);
                                form.handlebombclonning(clone);
                            }
                        }
                    }
                }
            }
            return explodedBombs;
        }

        public Vector2f AddOwner(string id)
        {
            this.PlayerId = id;
            var pos = this.possiblePlayerPos[0];
            possiblePlayerPos.RemoveAt(0);
            players.Add(new Player(id, PlayerName, pos));
            return pos;
        }

        public Vector2f? AddEnemy(string id, string name)
        {
            // Hacky, but this is the owner id and it cannot join twice.
            // TODO: fix this somehow in the future :))))
            if (this.PlayerId == id)
            {
                return null;
            }
            var pos = this.possiblePlayerPos[0];
            possiblePlayerPos.RemoveAt(0);
            players.Add(new Player(id, name, pos));

            return pos;
        }

        public Player? GetOwnerPlayer()
        {
            return players.SingleOrDefault(p => p.Id == PlayerId);
        }

        public Player? GetEnemyPlayer()
        {
            return players.SingleOrDefault(p => p.Id != PlayerId);
        }

        public List<Player> GetMovingPlayers()
        {
            return players
                .Where(p => p.Direction != Directions.Idle)
                .ToList();
        }

        public Player GetPlayerById(string id)
        {
            return players.Where(p => p.Id == id).First();
        }

        public Bomb? PlaceBomb(string playerId)
        {
            var player = players.Find(p => p.Id == playerId);
            if (player == null) return null;

            // Can't place a bomb while still moving
            if (player.Direction != Directions.Idle) return null;

            // can't place two bombs at the same spot
            foreach(var otherBomb in Bombs)
            {
                if (otherBomb.Position.Equals(player.Position))
                {
                    return null;
                }
            }

            var bomb = new Bomb(player.Position, player, player.BombExplosionRadius, 0);//BombFactory.GetBombInstance(player.BombType, player.Position, player, player.BombExplosionRadius);
            bomb.setExplosion(player.BombType);
            Bombs.Add(bomb);

            return bomb;
        }

        public void PerformAction(string playerId, string action)
        {
            var player = players.Find(p => p.Id == playerId);

            if (player == null) return;

            PlayerMovementFacade playerMovementFacade = new PlayerMovementFacade(player, action, Grid, this);

            if (playerMovementFacade.canSetDirection())
            {
                playerMovementFacade.movePlayer();
            }
        }

        public void MovePlayer(Player player)
        {
            player.Move();
            scoreEvents.Add(new Tuple<object, Player>(Constants.SCORE_STRATEGY_MOVEMENT, player));

            int gridIndex = GetGridIndex(player.Position);
            var cell = Grid[player.Position.Y,player.Position.X];
            
            if (cell.Type == BlockType.Fire)
            { player.IsAlive = false; }
            if (Powerups.ContainsKey(gridIndex))
            {
                Powerups[gridIndex].ApplyPowerUp(this, player);
                scoreEvents.Add(
                    new Tuple<object, Player>(
                            Powerups[gridIndex].GetType() is ScorePowerupStrategy ? 
                            Constants.SCORE_STRATEGY_SCORE_POWERUP : Constants.SCORE_STRATEGY_PICKUP_POWERUP,
                            player
                        )
                    );

                Powerups.Remove(gridIndex);
            }
            if (Bombtypes.ContainsKey(gridIndex))
            {
                Bombtypes[gridIndex].ChangeBombType(player);
                Bombtypes.Remove(gridIndex);
            }
        }

        public bool IsPositionValid(Vector2f pos)
        {
            return pos.X >= 0 
                && pos.Y >= 0 
                && pos.X <= GameDataSingleton.GetInstance().Width - 1 
                && pos.Y <= GameDataSingleton.GetInstance().Height - 1;
        }

        public void SetPlayerId(string id)
        {
            this.PlayerId = id;
        }

        public List<Bomb> getBombsByPlayerId(string id)
        {
            return Bombs.FindAll(bomb => bomb.Owner.Id == id);
        }

        public void setGameStatus(GameStatus gameStatus)
        {
            GameDataSingleton.GetInstance().SetCurrentGameStatus(gameStatus);
        }

        private void LoadMap()
        {
            var maplayout = Properties.Resources.Level1;
            GameDataSingleton.GetInstance().SetHeight(maplayout.Split("\r\n").Length);

            using (System.IO.StringReader reader = new System.IO.StringReader(maplayout))
            {
                string line = String.Empty;
                int currentRow = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    if (GameDataSingleton.GetInstance().Width == 0 || Grid == null)
                    {
                        GameDataSingleton.GetInstance().SetWidth(line.Length);

                        Grid = new Block[GameDataSingleton.GetInstance().Height, GameDataSingleton.GetInstance().Width];
                        ExplosionIntensity = new int[GameDataSingleton.GetInstance().Height, GameDataSingleton.GetInstance().Width];
                    }

                    char[] chararray = line.ToArray();
                    int currentCollum = 0;
                    foreach (char charele in chararray)
                    {
                        BlockType type = BlockType.Empty;
                        switch (charele)
                        {
                            case '#': // indestructable
                                type = BlockType.InDestructable;
                                break;
                            case 'c': // destructable
                                type = BlockType.Destructable;
                                break;
                            case '.':
                                type = BlockType.Empty;
                                break;
                            default:
                                possiblePlayerPos.Add(new Vector2f(currentCollum, currentRow));
                                break;
                        }
                        this.Grid[currentRow, currentCollum] = new Block(new Vector2f(currentCollum, currentRow), type);
                        currentCollum++;
                    }
                    currentRow++;
                }
                reader.Close();
            }
        }
    }
}
