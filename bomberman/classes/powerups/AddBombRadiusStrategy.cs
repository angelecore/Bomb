using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    internal class AddBombRadiusStrategy : IPowerup
    {
        public void ApplyPowerUp(GameState gameState, Player player)
        {
            player.AddNewStat(new PlayerStatsBuilder()
                .WithRadius(1)
                .Build()
            );
        }
    }
}
