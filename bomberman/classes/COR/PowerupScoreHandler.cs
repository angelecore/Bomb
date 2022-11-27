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
        public override object Handle(object request, Player player)
        {
            if (request is ScorePowerupStrategy && player is not null)
            {
                player.Score += 15;

                return null;
            }
            else
            {
                return base.Handle(request, player);
            }
        }
    }
}
