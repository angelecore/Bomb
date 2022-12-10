using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bomberman.classes;

namespace bomberman.classes.COR
{
    public class PowerupScoreHandler : ScoreHandler
    {
        public override object Handle(string eventsString, GameState gameState)
        {
            if (Utils.CheckConcatenatedEventsForScoreHasGivenEvent(eventsString, Constants.SCORE_STRATEGY_SCORE_POWERUP))
            {
                var player = gameState.GetPlayerById(Utils.GetConcatenatedEventsForScorePlayerId(eventsString, Constants.SCORE_STRATEGY_SCORE_POWERUP));
                player.Score += 15;
            }

            return base.Handle(eventsString, gameState);
        }
    }
}
