using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public interface IExplode
    {
        public List<Tuple<Vector2f, int>> ExplosionPossitions(Block[,] grid, Func<Vector2f, bool> isPositionValid, Bomb bomb);
    }

    class DynamiteExplosion : IExplode
    {
        public List<Tuple<Vector2f, int>> ExplosionPossitions(Block[,] grid, Func<Vector2f, bool> isPositionValid, Bomb bomb)
        {
            var positions = new List<Tuple<Vector2f, int>>();

            var start = bomb.Position;
            var range = bomb.Radius / 2;

            for (int i = -range; i <= range; i++)
            {
                for (int j = -range; j <= range; j++)
                {
                    Vector2f newPos = new Vector2f(start.X + i, start.Y + j);
                    // If the position is not valid or the block is indestructable - stop
                    if (!isPositionValid(newPos) || grid[newPos.Y, newPos.X].Type == BlockType.InDestructable)
                    {
                        continue;
                    }

                    positions.Add(new Tuple<Vector2f, int>(newPos, bomb.Radius * 2));
                }
            }
            return positions;
        }

    }
}
