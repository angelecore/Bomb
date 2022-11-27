using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.COR
{
    public class PowerupPickupScoreHandler : ScoreHandler
    {
        public override object Handle(object request, Player player)
        {
            if (request is IPowerup && request is not ScorePowerupStrategy && player is not null)
            {
                player.Score += 5;

                return null;
            }
            else
            {
                return base.Handle(request, player);
            }
        }
    }
}
