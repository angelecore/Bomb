using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public class ReversePowerupStrategy : IPowerup
    {
        public void ApplyPowerUp(GameState gameState, Player player)
        {
            gameState.ActivateReverseMode(player);
        }
    }
}
