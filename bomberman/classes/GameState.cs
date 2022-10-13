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

    internal class GameState
    {
        private GameStatus CurrentGameStatus;
        public string PlayerName { get; set; }
        public string? PlayerId { get; set; }

        public int Width;
        public int Height;

        public Block[,] Grid;

        public int[,] ExplosionIntensity;

        private List<Vector2f> possiblePlayerPos = new List<Vector2f>();

        private List<Player> players = new List<Player>();
        public List<Bomb> Bombs = new List<Bomb>();
        int maxPlayerCount;

        public GameState(string playerName, int maxPlayerCount = 2)
        {
            this.PlayerName = playerName;
            CurrentGameStatus = GameStatus.WaitingForPlayers;
            LoadMap();
            this.maxPlayerCount = maxPlayerCount;
        }

        private void ExplodeBomb(Bomb bomb)
        {
            Bombs.Remove(bomb);
            var directions = new List<Directions>() { Directions.Up, Directions.Down, Directions.Left, Directions.Right };
            for (int i = 0; i < bomb.Radius; i++)
            {
                for (int j = directions.Count - 1; j >= 0; j--)
                {
                    var dir = directions[j];
                    var vector = Utils.MultiplyVector(Utils.GetDirectionVector(dir), i);
                    var newPos = Utils.AddVectors(bomb.Position, vector);

                    // If the position is not valid or the block is indestructable - stop
                    if (!IsPositionValid(newPos) || Grid[newPos.Y, newPos.X].Type == BlockType.InDestructable)
                    {
                        directions.RemoveAt(j);
                        continue;
                    }

                    var cell = Grid[newPos.Y, newPos.X];
                    ExplosionIntensity[newPos.Y, newPos.X] = (bomb.Radius - i) * 5;

                    // If another bomb is also is this direction, then also explode this bomb next tic.
                    bool bombReached = false;
                    foreach (var anotherBomb in Bombs)
                    {
                        if (anotherBomb.Position.Equals(cell.Position))
                        {
                            anotherBomb.Timer = 0;
                            bombReached = true;
                        }
                    }

                    foreach(var player in players)
                    {
                        if (player.Position.Equals(cell.Position))
                        {
                            player.IsAlive = false;
                        }
                    }

                    // Destroy this block and stop the explosion in this direction
                    if (cell.Type == BlockType.Destructable || bombReached)
                    {
                        cell.Type = BlockType.Empty;
                        directions.RemoveAt(j);
                    }  
                }
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

        public List<Bomb> UpdateBombTimers(double miliSeconds)
        {
            var explodedBombs = new List<Bomb>();
            for(int i = Bombs.Count - 1; i >= 0; i--)
            {
                var bomb = Bombs[i];
                // 1 tick
                bomb.Timer -= (float)miliSeconds * 0.001f;
                if (bomb.Timer < 1)
                {
                    // boom
                    ExplodeBomb(bomb);
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
            players.Add(new Player(id, pos));
            return pos;
        }

        public Vector2f? AddEnemy(string id)
        {
            // Hacky, but this is the owner id and it cannot join twice.
            // TODO: fix this somehow in the future :))))
            if (this.PlayerId == id)
            {
                return null;
            }
            var pos = this.possiblePlayerPos[0];
            possiblePlayerPos.RemoveAt(0);
            players.Add(new Player(id, pos));

            return pos;
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
            var bomb = new Bomb(player.Position);
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

            if (Grid[nextPos.Y, nextPos.X].Type != BlockType.Empty && player.Direction == Directions.Idle)
            {
                return;
            }

            // if the player was already moving, then update its position
            if (player.Direction != Directions.Idle && player.Direction != newDirection)
            {
                player.Move();
            }

            player.SetDirection(newDirection);    
        }

        public bool IsPositionValid(Vector2f pos)
        {
            return pos.X >= 0 && pos.Y >= 0 && pos.X <= Width - 1 && pos.Y <= Height - 1;
        }

        public void SetPlayerId(string id)
        {
            this.PlayerId = id;
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
