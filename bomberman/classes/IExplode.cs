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

    class BasicExplosion : IExplode
    {
        public List<Tuple<Vector2f, int>> ExplosionPossitions(Block[,] grid, Func<Vector2f, bool> isPositionValid, Bomb bomb)
        {
            var positions = new List<Tuple<Vector2f, int>>();

            var directions = new List<Directions>() { Directions.Up, Directions.Down, Directions.Left, Directions.Right };
            for (int i = 0; i < bomb.Radius; i++)
            {
                for (int j = directions.Count - 1; j >= 0; j--)
                {
                    var dir = directions[j];
                    var vector = Utils.MultiplyVector(Utils.GetDirectionVector(dir), i);
                    var newPos = Utils.AddVectors(bomb.Position, vector);

                    // If the position is not valid or the block is indestructable - stop
                    if (!isPositionValid(newPos) || grid[newPos.Y, newPos.X].Type == BlockType.InDestructable)
                    {
                        directions.RemoveAt(j);
                        continue;
                    }
                    if (positions.Contains(new Tuple<Vector2f, int>(newPos, (bomb.Radius - i) * 5)))
                        continue;
                    positions.Add(new Tuple<Vector2f, int>(newPos, (bomb.Radius - i) * 5));
                }
            }

            return positions;
        }

    }

    class FireExplosion : IExplode
    {
        public List<Tuple<Vector2f, int>> ExplosionPossitions(Block[,] grid, Func<Vector2f, bool> isPositionValid, Bomb bomb)
        {
            var positions = new List<Tuple<Vector2f, int>>();

            var directions = new List<Directions>() { Directions.Up, Directions.Down, Directions.Left, Directions.Right };
            for (int i = 0; i < bomb.Radius*2; i++)
            {
                for (int j = directions.Count - 1; j >= 0; j--)
                {
                    var dir = directions[j];
                    var vector = Utils.MultiplyVector(Utils.GetDirectionVector(dir), i);
                    var newPos = Utils.AddVectors(bomb.Position, vector);

                    // If the position is not valid or the block is indestructable - stop
                    if (!isPositionValid(newPos) || grid[newPos.Y, newPos.X].Type == BlockType.InDestructable)
                    {
                        directions.RemoveAt(j);
                        continue;
                    }
                    if (positions.Contains(new Tuple<Vector2f, int>(newPos, (bomb.Radius - i) * 5)))
                        continue;
                    positions.Add(new Tuple<Vector2f, int>(newPos, (bomb.Radius - i) * 5));
                }
            }

            return positions;
        }

    }

    class ClusterExplosion : IExplode
    {
        public List<Tuple<Vector2f, int>> ExplosionPossitions(Block[,] grid, Func<Vector2f, bool> isPositionValid, Bomb bomb)
        {
            var positions = new List<Tuple<Vector2f, int>>();

            var directions = new List<Directions>() { Directions.Up, Directions.Down, Directions.Left, Directions.Right };
            for (int i = 0; i < 2; i++)
            {
                for (int j = directions.Count - 1; j >= 0; j--)
                {
                    var dir = directions[j];
                    var vector = Utils.MultiplyVector(Utils.GetDirectionVector(dir), i);
                    var newPos = Utils.AddVectors(bomb.Position, vector);

                    // If the position is not valid or the block is indestructable - stop
                    if (!isPositionValid(newPos) || grid[newPos.Y, newPos.X].Type == BlockType.InDestructable)
                    {
                        directions.RemoveAt(j);
                        continue;
                    }
                    if (positions.Contains(new Tuple<Vector2f, int>(newPos, (bomb.Radius - i) * 5)))
                        continue;
                    positions.Add(new Tuple<Vector2f, int>(newPos, (2 - i) * 5));
                }
            }

            return positions;
        }

    }
}
