using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bomberman.classes.mediator;

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

        public virtual object Handle(string eventsString, IGameManager gameManager)
        {
            if (successor != null)
            {
                return successor.Handle(eventsString, gameManager);
            }
            else
            {
                return null;
            }
        }
    }
}
