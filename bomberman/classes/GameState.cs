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

        public const int Width = 7;
        public const int Height = 7;

        public Block[,] Grid = new Block[Height, Width];

        private List<Vector2f> possiblePlayerPos = new List<Vector2f>();

        private List<Player> players = new List<Player>();

        public GameState(string playerName)
        {
            this.PlayerName = playerName;
            LoadMap();
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

            using (System.IO.StringReader reader = new System.IO.StringReader(maplayout))
            {
                string line = String.Empty;
                int currentRow = 0;
                while ((line = reader.ReadLine()) != null)
                {
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
                                possiblePlayerPos.Add(new Vector2f(currentRow, currentCollum));
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
