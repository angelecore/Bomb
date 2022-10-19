using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public class PlayerTemporaryStats
    {
        public string Id { get; set; }
        public float ActiveTimer { get; set; }

        public int AddSpeedAmount { get; set; }
        public bool Applied { get; set; }

        public PlayerTemporaryStats(float activeTimer, int addSpeedAmount)
        {
            Id = Guid.NewGuid().ToString();
            ActiveTimer = activeTimer;
            AddSpeedAmount = addSpeedAmount;
            Applied = false;
        }
    }
}
