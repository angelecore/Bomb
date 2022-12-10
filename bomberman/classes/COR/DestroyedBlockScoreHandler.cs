using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.COR
{
    public class DestroyedBlockScoreHandler : ScoreHandler
    {
        public override object Handle(string eventsString, GameState gameState)
        {
            if (Utils.CheckConcatenatedEventsForScoreHasGivenEvent(eventsString, Constants.SCORE_STRATEGY_DESTROYED_BLOCK))
            {
                var player = gameState.GetPlayerById(Utils.GetConcatenatedEventsForScorePlayerId(eventsString, Constants.SCORE_STRATEGY_DESTROYED_BLOCK));
                player.Score += 3;
            }

            return base.Handle(eventsString, gameState);
        }
    }
}
