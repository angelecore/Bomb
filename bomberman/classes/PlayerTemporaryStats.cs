﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public class PlayerTemporaryStats
    {
        public string Id { get; set; }
        public float ?ActiveTimer { get; set; }

        public int ?AddSpeedAmount { get; set; }
        public int? AddRadiusAmount { get; set; }
        public bool Applied { get; set; }

        public PlayerTemporaryStats()
        {
            Id = Guid.NewGuid().ToString();
            Applied = false;
        }

        public PlayerTemporaryStats WithRadius(int radius)
        {
            AddRadiusAmount = radius;
            return this;
        }

        public PlayerTemporaryStats WithTimer(float timer)
        {
            ActiveTimer = timer;
            return this;
        }

        public PlayerTemporaryStats WithAddSpeed(int speed)
        {
            this.AddSpeedAmount = speed;
            return this;
        }

        public override string ToString()
        {

            return String.Format("Effect timer: {0} | Add {1}", ActiveTimer == null ? "Always" : (int)ActiveTimer, GetEffectString());
        }

        private string GetEffectString()
        {
            if (AddSpeedAmount != null)
            {
                return String.Format("speed");
            }

            return String.Format("radius");
        }
    }
}