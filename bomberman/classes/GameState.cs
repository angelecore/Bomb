using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public enum GameStatus
    {
        WaitingForPlayers,
        InProgress,
        Lost,
        Tie,
        Won
    }

    public class GameState
    {

        private GameStatus CurrentGameStatus;
        public string PlayerName { get; set; }
        public string? PlayerId { get; set; }

        public int Width;
        public int Height;

        public Block[,] Grid;

        public List<Fire> FireList = new List<Fire>();

        private BombType allBombType;

        public int[,] ExplosionIntensity;

        private List<Vector2f> possiblePlayerPos = new List<Vector2f>();

        private List<Player> players = new List<Player>();
        public List<Bomb> Bombs = new List<Bomb>();

        // key is the index of the Grid tile
        public Dictionary<int, IPowerup> Powerups = new Dictionary<int, IPowerup>();
        public Dictionary<int, IBombtype> Bombtypes = new Dictionary<int, IBombtype>();
        int maxPlayerCount;

        public GameState(string playerName, int maxPlayerCount = 2)
        {
            this.PlayerName = playerName;
            CurrentGameStatus = GameStatus.WaitingForPlayers;
            LoadMap();
            this.maxPlayerCount = maxPlayerCount;
        }

        public int GetGridIndex(Vector2f position)
        {
            return position.Y * Width + position.X;
        }

        private void RemoveBox(Player responsiblePlayer, Vector2f position)
        {
            var cell = Grid[position.Y, position.X];
            //if(responsiblePlayer.BombType == BombType.Fire)
                //cell.Type = BlockType.Fire;
            //else
                cell.Type = BlockType.Empty;
            responsiblePlayer.Score++;

            int gridIndex = GetGridIndex(position);
            var temp = new List<BombType> { BombType.Fire, BombType.Fire, BombType.Fire };
            // add-on powerup logic!
            var random = new Random();
            // Every Sixth crate is a bomb change
            if (responsiblePlayer.Score % 6 == 0)
            {
                Bombtypes.Add(gridIndex, new ChangeBombTypeStrategy(temp[random.Next(0, 2)]));
                //Bombtypes.Add(gridIndex, new ChangeBombTypeStrategy(BombType.Dynamite));
                return;
            }

            // Every 5th crate is a speed power up
            if (responsiblePlayer.Score % 5 == 0)
            {
                Powerups.Add(gridIndex, new SpeedPowerupStrategy());
                return;
            }

            // Every third crate is a power up
            if (responsiblePlayer.Score % 3 == 0)
            {
                Powerups.Add(gridIndex, new AddBombRadiusStrategy());
            }
        }

        public GameStatus CheckGameStatus()
        {
            if (CurrentGameStatus == GameStatus.WaitingForPlayers)
            {
                if (players.Count == maxPlayerCount)
                {
                    CurrentGameStatus = GameStatus.InProgress;
                }
            }
            else
            {
                if (players.Count == 0)
                {
                    CurrentGameStatus = GameStatus.Tie;
                }

                if (players.Count == 1)
                {
                    CurrentGameStatus = players.First().Id == PlayerId ? GameStatus.Won : GameStatus.Lost;
                }
            }

            return CurrentGameStatus;
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

        public void UpdateTick(float miliSeconds)
        {
            foreach(var player in players)
            {
                player.UpdateTemporaryStats(miliSeconds);
            }
        }

        public void RemoveExplodedTiles(List<Tuple<Vector2f, int>> cells, Player owner, Fire fire)
        {
            foreach(var cell in cells)
            {
                var pos = cell.Item1;
                ExplosionIntensity[pos.Y, pos.X] = cell.Item2;

                // If another bomb is also is this direction, then also explode this bomb next tic.
                bool bombReached = false;
                foreach (var anotherBomb in Bombs)
                {
                    if (anotherBomb.Position.Equals(pos))
                    {
                        anotherBomb.Timer = 0;
                        bombReached = true;
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
                    tyle.firerefrence = fire; 
                    tyle.Type = BlockType.Fire;
                }
                 //Destroy this block and stop the explosion in this direction
                if (Grid[pos.Y, pos.X].Type == BlockType.Destructable || bombReached)
                {
                    RemoveBox(owner, pos);
                }
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
                    if (bomb.Owner.BombType == BombType.Fire)
                    {
                        fire = new Fire(DateTime.Now.Millisecond.ToString());
                        FireList.Add(fire);
                    }
                    Bombs.RemoveAt(i);
                    RemoveExplodedTiles(cells, bomb.Owner, fire);
                    explodedBombs.Add(bomb);
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

        public List<Player> GetMovingPlayers()
        {
            return players
                .Where(p => p.Direction != Directions.Idle)
                .ToList();
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

            var bomb = BombFactory.GetBombInstance(player.BombType, player.Position, player, player.BombExplosionRadius);
            Bombs.Add(bomb);

            return bomb;
        }

        public void PerformAction(string playerId, string action)
        {
            var player = players.Find(p => p.Id == playerId);

            if (player == null) return;

            Directions newDirection = Directions.Idle;

            switch (action)
            {
                case "Up":
                    newDirection = Directions.Up;
                    break;
                case "Down":
                    newDirection = Directions.Down;
                    break;
                case "Left":
                    newDirection = Directions.Left;
                    break;
                case "Right":
                    newDirection = Directions.Right;
                    break;
            }

            var dirVec = Utils.GetDirectionVector(newDirection);

            if (player.Direction != Directions.Idle)
            {
                var currentInvVec = Utils.MultiplyVector(
                    Utils.GetDirectionVector(player.Direction),
                    -1
                );

                // while the player hasnt stopped moving, you can only go backwards.
                if (!currentInvVec.Equals(dirVec))
                {
                    return;
                }    
            }

         
            var nextPos = Utils.AddVectors(player.Position, dirVec);
            
            if (!IsPositionValid(nextPos))
            {
                return;
            }

            if (Grid[nextPos.Y, nextPos.X].Type != BlockType.Empty && Grid[nextPos.Y, nextPos.X].Type != BlockType.Fire && player.Direction == Directions.Idle)
            {
                return;
            }

            // if the player was already moving, then update its position
            if (player.Direction != Directions.Idle && player.Direction != newDirection)
            {
                MovePlayer(player);
            }

            player.SetDirection(newDirection);    
        }

        public void MovePlayer(Player player)
        {
            player.Move();

            int gridIndex = GetGridIndex(player.Position);
            var cell = Grid[player.Position.Y,player.Position.X];
            
            if (cell.Type == BlockType.Fire)
            { player.IsAlive = false; }
            if (Powerups.ContainsKey(gridIndex))
            {
                Powerups[gridIndex].ApplyPowerUp(this, player);
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
            return pos.X >= 0 && pos.Y >= 0 && pos.X <= Width - 1 && pos.Y <= Height - 1;
        }

        public void SetPlayerId(string id)
        {
            this.PlayerId = id;
        }

        public List<Bomb> getBombsByPlayerId(string id)
        {
            return Bombs.FindAll(bomb => bomb.Owner.Id == id);
        }

        private void LoadMap()
        {
            var maplayout = Properties.Resources.Level1;

            Height = maplayout.Split("\r\n").Length;

            using (System.IO.StringReader reader = new System.IO.StringReader(maplayout))
            {
                string line = String.Empty;
                int currentRow = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    if (Width == 0)
                    {
                        Width = line.Length;
                        Grid = new Block[Height, Width];
                        ExplosionIntensity = new int[Height, Width];
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
