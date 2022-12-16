using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bomberman.classes.mediator;

namespace bomberman.classes.COR
{
    public class DestroyedBlockScoreHandler : ScoreHandler
    {
        public override object Handle(string eventsString, IGameManager gameManager)
        {
            if (Utils.CheckConcatenatedEventsForScoreHasGivenEvent(eventsString, Constants.SCORE_STRATEGY_DESTROYED_BLOCK))
            {
                var player = gameManager.GetPlayerById(Utils.GetConcatenatedEventsForScorePlayerId(eventsString, Constants.SCORE_STRATEGY_DESTROYED_BLOCK));
                player.Score += 3;
            }

            return base.Handle(eventsString, gameManager);
        }
    }
}
