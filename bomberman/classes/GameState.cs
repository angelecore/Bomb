using bomberman.classes.facade;
using bomberman.classes.memento;
using bomberman.classes.Timers;
using bomberman.classes.Compositetree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bomberman.classes.mediator;

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

        public List<Tuple<object, Player>> scoreEvents = new List<Tuple<object, Player>>();

        // key is the index of the Grid tile
        public Dictionary<int, IPowerup> Powerups = new Dictionary<int, IPowerup>();
        public Dictionary<int, IBombtype> Bombtypes = new Dictionary<int, IBombtype>();

        public List<RegenerationTimer> RegenTimer = new List<RegenerationTimer>();

        private float secondTimer = 1.0f;

        private IGameManager _gameManager;
        private int spawnIndex = 0;

        public GameState(string playerName, ConcreteObserver gameUI, int maxPlayerCount = 2)
        {
            // init game manager (Mediator pattern)
            _gameManager = new GameManager(this, gameUI);

            this.PlayerName = playerName;
            GameDataSingleton.GetInstance().SetCurrentGameStatus(GameStatus.WaitingForPlayers);
            LoadMap();
            GameDataSingleton.GetInstance().SetMaxPlayerCount(maxPlayerCount);
        }

        public IGameManager GetGameManager()
        {
            return _gameManager;
        }

        public void killPlayer(Vector2f pos)
        {
            foreach (var player in players)
            {
                if (player.Position.Equals(pos))
                {
                    player.IsAlive = false;
                }
            }
        }

        public int GetGridIndex(Vector2f position)
        {
            return position.Y * GameDataSingleton.GetInstance().Width + position.X;
        }

        public void ActivateReverseMode(Player initiator)
        {
            _gameManager.ApplyReversePlayerMode(initiator);
        }

        private void RemoveBox(Player responsiblePlayer, Vector2f position)
        {
            var cell = Grid[position.Y, position.X];
            //if(responsiblePlayer.BombType == BombType.Fire)
                //cell.Type = BlockType.Fire;
            //else
            bool flag = false;
            if (cell.Type == BlockType.Regenerating)
            {
                if(responsiblePlayer.BombType != BombType.Fire)
                    RegenTimer.Add(new RegenerationTimer(8, cell));

            }
            else if (cell.Type != BlockType.Empty)
            {
                scoreEvents.Add(new Tuple<object, Player>(Constants.SCORE_STRATEGY_DESTROYED_BLOCK, responsiblePlayer));
                flag = true;
            }
            if (responsiblePlayer.BombType != BombType.Fire)
                cell.ChangeState(BlockType.Empty);
            else
                cell.ChangeState(BlockType.Fire);
            int gridIndex = GetGridIndex(position);
            // var temp = new List<BombType> { BombType.Fire, BombType.Fire, BombType.Fire };
            // add-on powerup logic!
            // var random = new Random();
            if (flag)
            {
                // There's only 10 types of object that can spawn,
                // if more is added you need to increase this number
                spawnIndex = (spawnIndex + 1) % 9;

                if (spawnIndex == 1)
                {
                    AddBombType(gridIndex, new ChangeBombTypeStrategy(BombType.Cluster));
                    return;
                }

                if (spawnIndex == 2)
                {
                    AddBombType(gridIndex, new ChangeBombTypeStrategy(BombType.Fire));
                    return;
                }

                if (spawnIndex == 3)
                {
                    AddBombType(gridIndex, new ChangeBombTypeStrategy(BombType.Dynamite));
                    return;
                }

                IPowerup ?newPowerup = PowerupFactory.GetPowerupInstance(spawnIndex);
                if (newPowerup != null)
                {
                    if (!Powerups.ContainsKey(gridIndex))
                    {
                        Powerups.Add(gridIndex, newPowerup);
                    }
                }
            }
            //return;
        }

        public void AddBombType(int gridIndex, IBombtype bombType)
        {
            if (!Bombtypes.ContainsKey(gridIndex))
            {
                Bombtypes.Add(gridIndex, bombType);
            }
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

        public void RemoveExplodedTiles(List<Tuple<Vector2f, int>> cells, Player owner, FireController fire)
        {
            foreach(var cell in cells)
            {
                var pos = cell.Item1;
                ExplosionIntensity[pos.Y, pos.X] = cell.Item2;

                _gameManager.ExplodePosition(pos);

                if (fire != null && owner.BombType == BombType.Fire)
                {
                    FireController clone = (FireController) fire.Clone();
                    clone.BlockX = pos.X;
                    clone.BlockY = pos.Y;
                    FireControllerList.Add(clone);
                }
                
                 // Destroy this block
                 RemoveBox(owner, pos);
            }
        }

        public List<Tuple<Vector2f, int>> ExplodeBomb(Bomb bomb)
        {
            Fire fire = null;
            List<Tuple<Vector2f, int>> cells;
            if (bomb is ClusterBomb)
            {
                cells = ((ClusterBomb)bomb).GetExplosionPositions(Grid, (pos) => IsPositionValid(pos));
            }
            else
            {
                cells = bomb.GetExplosionPositions(Grid, (pos) => IsPositionValid(pos));
            }

            FireController controller = null;
            if (bomb.Owner.BombType == BombType.Fire)
            {
                fire = new Fire(DateTime.Now.Millisecond.ToString());
                controller = new FireController(fire.ID, fire, bomb.Position.X, bomb.Position.Y);
                FireList.Add(fire);
            }

            RemoveExplodedTiles(cells, bomb.Owner, controller);
            return cells;
        }

        public Vector2f AddOwner(string id)
        {
            this.PlayerId = id;
            var pos = this.possiblePlayerPos[0];
            possiblePlayerPos.RemoveAt(0);
            players.Add(new Player(id, PlayerName, pos, _gameManager));
            _gameManager.RegisterPlayer((players.Last()));
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
            players.Add(new Player(id, name, pos, _gameManager));
            _gameManager.RegisterPlayer((players.Last()));

            return pos;
        }

        public Player? GetOwnerPlayer()
        {
            return players.SingleOrDefault(p => p.Id == PlayerId);
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

        public void PlaceBomb(string playerId)
        { 
            _gameManager.PlaceBomb(playerId);
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

        public void setGameStatus(GameStatus gameStatus)
        {
            GameDataSingleton.GetInstance().SetCurrentGameStatus(gameStatus);
        }

        private void LoadMap()
        {
            var maplayout = Properties.Resources.Level3;
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
                            case 'r':
                                type = BlockType.Regenerating;
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
