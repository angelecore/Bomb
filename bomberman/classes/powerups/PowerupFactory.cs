﻿using System;
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
            if (playerScore == 4)
            {
                return new AddBombRadiusStrategy();
            }

            if (playerScore == 5)
            {
                return new ScorePowerupStrategy();
            }

            if (playerScore == 6)
            {
                return new SpeedPowerupStrategy();
            }

            if (playerScore == 7)
            {
                return new FreezeSpeedPowerupStrategy();
            }

            if (playerScore == 8)
            {
                return new ReversePowerupStrategy();
            }
            return null;
        }
    }
}
