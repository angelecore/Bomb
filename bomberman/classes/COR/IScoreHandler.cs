using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bomberman.classes.mediator;

namespace bomberman.classes.COR
{
    public interface IScoreHandler
    {
        IScoreHandler setNext(IScoreHandler handler);

        object Handle(string eventsString, IGameManager gameManager);
    }
}
