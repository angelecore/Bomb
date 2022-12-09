using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    internal class PlayerStatsBuilder
    {
        private PlayerTemporaryStats _stats  = new PlayerTemporaryStats();

        public PlayerStatsBuilder WithRadius(int radius)
        {
            _stats.AddRadiusAmount = radius;
            return this;
        }

        public PlayerStatsBuilder WithTimer(float timer)
        {
            _stats.ActiveTimer = timer;
            return this;
        }

        public PlayerStatsBuilder WithAddSpeed(int speed)
        {
            _stats.AddSpeedAmount = speed;
            return this;
        }

        public PlayerTemporaryStats Build() => _stats;

    }
}
