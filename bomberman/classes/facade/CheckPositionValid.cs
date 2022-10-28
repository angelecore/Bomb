using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.facade
{
    public class CheckPositionValid : IValidator
    {
        int width { get; set; }
        int height { get; set; }
        public CheckPositionValid(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public bool IsValid(Player player, Vector2f dirVector)
        {
            var nextPosition = Utils.AddVectors(player.Position, dirVector);

            return nextPosition.X >= 0 && nextPosition.Y >= 0 && nextPosition.X <= width - 1 && nextPosition.Y <= height - 1;
        }
    }
}
