using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.COR
{
    public class DestroyedBlockScoreHandler : ScoreHandler
    {
        public override object Handle(object request, Player player)
        {
            if (request is BlockType && player is not null)
            {
                player.Score += 3;

                return null;
            }
            else
            {
                return base.Handle(request, player);
            }
        }
    }
}
