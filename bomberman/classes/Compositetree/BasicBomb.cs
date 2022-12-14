/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.Compositetree
{
    public class BasicBomb : Component
    {

        public string Id { get; set; }
        public Vector2f Position { get; set; }
        public float Timer { get; set; }
        public int Radius { get; set; }
        public Player Owner { get; set; }
        

        public object Clone(Bomb bomb, Vector2f position)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<Vector2f, int>> GetExplosionPositions(Block[,] grid, Func<Vector2f, bool> isPositionValid)
        {
            var positions = new List<Tuple<Vector2f, int>>();

            var directions = new List<Directions>() { Directions.Up, Directions.Down, Directions.Left, Directions.Right };
            for (int i = 0; i < Radius; i++)
            {
                for (int j = directions.Count - 1; j >= 0; j--)
                {
                    var dir = directions[j];
                    var vector = Utils.MultiplyVector(Utils.GetDirectionVector(dir), i);
                    var newPos = Utils.AddVectors(Position, vector);


                    // If the position is not valid or the block is indestructable - stop
                    if (!isPositionValid(newPos) || grid[newPos.Y, newPos.X].Type == BlockType.InDestructable)
                    {
                        directions.RemoveAt(j);
                        continue;
                    }
                    if (positions.Contains(new Tuple<Vector2f, int>(newPos, (Radius - i) * 5)))
                        continue;
                    positions.Add(new Tuple<Vector2f, int>(newPos, (Radius - i) * 5));
                }
            }

            return positions;
        }
    }
}
*/