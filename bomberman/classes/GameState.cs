using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    internal class GameState
    {
        public string PlayerName { get; set; }
        public string? PlayerId { get; set; }

        public int Width;
        public int Height;

        public Block[,] Grid;

        public int[,] ExplosionIntensity;

        private List<Vector2f> possiblePlayerPos = new List<Vector2f>();

        private List<Player> players = new List<Player>();
        public List<Bomb> Bombs = new List<Bomb>();

        public GameState(string playerName)
        {
            this.PlayerName = playerName;
            LoadMap();
        }

 
        private void ExplodeBomb(Bomb bomb)
        {
            var directions = new List<Directions>() { Directions.Up, Directions.Down, Directions.Left, Directions.Right };
            for (int i = 0; i < bomb.Radius; i++)
            {
                for (int j = directions.Count - 1; j >= 0; j--)
                {
                    var dir = directions[j];
                    var vector = Utils.MultiplyVector(Utils.GetDirectionVector(dir), i);
                    var newPos = Utils.AddVectors(bomb.Position, vector);
                    if (!IsPositionValid(newPos))
                    {
                        directions.RemoveAt(j);
                        continue;
                    }

                    var cell = Grid[newPos.Y, newPos.X];
                    if (cell.Type == BlockType.InDestructable)
                    {
                        directions.RemoveAt(j);
                        continue;
                    }

                    ExplosionIntensity[newPos.Y, newPos.X] = bomb.Radius - i;
                    if (cell.Type == BlockType.Empty)
                    {
                        continue;
                    }
                   
                    if (cell.Type == BlockType.Destructable)
                    {
                        cell.Type = BlockType.Empty;
                    }


                    directions.RemoveAt(j);
                }
            }
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
                    Bombs.RemoveAt(i);
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
