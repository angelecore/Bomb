using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    internal class ClusterBomb : Bomb
    {
        public ClusterBomb(Vector2f position, Player owner, int radius, int generation) : base(position, owner, radius, generation)
        {
        }

        public override object Clone(Bomb bomb, Vector2f position, int generation)
        {
            ClusterBomb temp = new ClusterBomb(position, bomb.Owner, bomb.Radius, generation);
            temp.Timer = 2f;
            return temp;
        }

        public override List<Tuple<Vector2f, int>> GetExplosionPositions(Block[,] grid, Func<Vector2f, bool> isPositionValid)
        {
            var positions = new List<Tuple<Vector2f, int>>();

            var directions = new List<Directions>() { Directions.Up, Directions.Down, Directions.Left, Directions.Right };
            for (int i = 0; i < 2; i++)
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

                    positions.Add(new Tuple<Vector2f, int>(newPos, (2 - i) * 5));
                }
            }

            return positions;
        }

    }
}
