using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.COR
{
    public class MovementScoreHandler : ScoreHandler
    {
        public override object Handle(object request, Player player)
        {
            if ((request as string) == Constants.SCORE_STRATEGY_MOVEMENT && player is not null)
            {
                player.Score += 1;

                return null;
            }
            else
            {
                return base.Handle(request, player);
            }
        }
    }
}
