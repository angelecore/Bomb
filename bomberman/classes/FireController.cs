using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public class FireController : ICloneable
    {
        public Fire fire { get; set; }
        public int BlockX { get; set; }
        public int BlockY { get; set; }
        public string ID { get; set; }

        public FireController(string id , Fire fire, int x, int y)
        {
            this.fire = fire;
            this.BlockX = x;
            this.BlockY = y;
            this.ID = id;
        }

        public object Clone()
        {
            return this.MemberwiseClone() as FireController;
        }
    }
}
