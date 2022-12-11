using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public class FreezeSpeedPowerupStrategy : IPowerup
    {
        public void ApplyPowerUp(GameState gameState, Player player)
        {
            player.AddNewStat(new PlayerStatsBuilder()
                .WithAddSpeed(-10)
                .WithTimer(5)
                .Build()
            );
        }
    }
}
