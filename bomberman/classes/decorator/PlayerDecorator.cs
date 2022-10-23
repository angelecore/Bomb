using bomberman.client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public abstract class PlayerDecorator : IPlayerDecorator
    {
        protected IPlayerDecorator basePlayerDecorator;

        public PlayerDecorator(IPlayerDecorator playerDecorator)
        {
            this.basePlayerDecorator = playerDecorator;
        }

        public virtual IPlayerDecorator decorate()
        {
            return basePlayerDecorator.decorate();
        }

        public string getName()
        {
            return basePlayerDecorator.getName();
        }

        public int getPlacedBombs()
        {
            return basePlayerDecorator.getPlacedBombs();
        }

        public Point getPosition()
        {
            return basePlayerDecorator.getPosition();
        }

        public Image getPlayerSprite()
        {
            return basePlayerDecorator.getPlayerSprite();
        }
    }
}
