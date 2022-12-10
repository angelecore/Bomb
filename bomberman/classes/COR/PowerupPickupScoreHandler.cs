using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.COR
{
    public class PowerupPickupScoreHandler : ScoreHandler
    {
        public override object Handle(string eventsString, GameState gameState)
        {
            if (Utils.CheckConcatenatedEventsForScoreHasGivenEvent(eventsString, Constants.SCORE_STRATEGY_PICKUP_POWERUP))
            {
                var player = gameState.GetPlayerById(Utils.GetConcatenatedEventsForScorePlayerId(eventsString, Constants.SCORE_STRATEGY_PICKUP_POWERUP));
                player.Score += 5;
            }

            return base.Handle(eventsString, gameState);
        }
    }
}
