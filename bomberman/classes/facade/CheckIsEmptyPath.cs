using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.facade
{

    public class CheckIsEmptyPath : IValidator
    {
        Block[,] grid { set; get; }
        public CheckIsEmptyPath(Block[,] grid)
        {
            this.grid = grid;
        }

        public bool IsValid(Player player, Vector2f dirVector)
        {
            var nextPosition = Utils.AddVectors(player.Position, dirVector);

            return grid[nextPosition.Y, nextPosition.X].Type == BlockType.Empty || grid[nextPosition.Y, nextPosition.X].Type == BlockType.Fire;
        }
    }
}
