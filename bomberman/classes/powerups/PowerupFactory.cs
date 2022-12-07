using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public static class PowerupFactory
    {
        public static IPowerup? GetPowerupInstance(int playerScore)
        {
            if (playerScore % 7 == 0)
            {
                return new ScorePowerupStrategy();
            }

            if (playerScore % 5 == 0)
            {
                return new SpeedPowerupStrategy();
            }

            if (playerScore % 7 == 0)
            {
                return new FreezeSpeedPowerupStrategy();
            }

            if (playerScore % 3 == 0)
            {
                return new AddBombRadiusStrategy();
            }

            return null;
        }
    }
}
