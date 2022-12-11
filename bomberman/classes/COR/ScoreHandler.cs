using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.COR
{
    public abstract class ScoreHandler : IScoreHandler
    {
        private IScoreHandler successor;

        public IScoreHandler setNext(IScoreHandler scoreHandler)
        {
            this.successor = scoreHandler;

            return scoreHandler;
        }

        public virtual object Handle(string eventsString, GameState gameState)
        {
            if (successor != null)
            {
                return successor.Handle(eventsString, gameState);
            }
            else
            {
                return null;
            }
        }
    }
}
