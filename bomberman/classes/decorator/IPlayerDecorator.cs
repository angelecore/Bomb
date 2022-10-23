using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public interface IPlayerDecorator
    {
        public IPlayerDecorator decorate();

        public string getName();

        public int getPlacedBombs();

        public Point getPosition();

        public Image getPlayerSprite();
    }
}
