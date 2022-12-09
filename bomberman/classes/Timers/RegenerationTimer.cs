using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.Timers
{
    public class RegenerationTimer
    {
        public float Timer { get; set; }
        public Block RegeneratingBlock { get; set; }

        public RegenerationTimer(float timer, Block regeneratingBlock)
        {
            Timer = timer;
            RegeneratingBlock = regeneratingBlock;
        }   
    }
}
