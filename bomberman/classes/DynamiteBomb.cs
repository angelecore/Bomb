//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace bomberman.classes
//{
//    public class DynamiteBomb : Bomb
//    {
//        public DynamiteBomb(Vector2f position, Player owner, int radius, int generation) : base(position, owner, radius, generation)
//        {
//        }

//        public override object Clone(Bomb bomb, Vector2f position, int generation)
//        {
//            throw new NotImplementedException();
//        }

//        public override List<Tuple<Vector2f, int>> GetExplosionPositions(Block[,] grid, Func<Vector2f, bool> isPositionValid)
//        {
//            var positions = new List<Tuple<Vector2f, int>>();

//            var start = Position;
//            var range = Radius / 2;

//            for (int i = -range; i <= range; i++)
//            {
//                for (int j = -range; j <= range; j++)
//                {
//                    Vector2f newPos = new Vector2f(start.X + i, start.Y + j);
//                    // If the position is not valid or the block is indestructable - stop
//                    if (!isPositionValid(newPos) || grid[newPos.Y, newPos.X].Type == BlockType.InDestructable)
//                    {
//                        continue;
//                    }

//                    positions.Add(new Tuple<Vector2f, int>(newPos, Radius * 2));
//                }
//            }
//            return positions;
//        }
//    }
//}
