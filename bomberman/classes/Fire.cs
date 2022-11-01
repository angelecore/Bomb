using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public class Fire
    {
        public float Timer;
        public string ID { get; set; }

        public Fire(string id)
        {
            this.Timer = 5f;
            this.ID = id;
        }

        public Fire ShallowClone()
        {
            return this;
        }
    }
}
