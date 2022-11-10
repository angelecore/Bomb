using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.adapter
{
    public interface ILogger
    {
        public void output(List<string> logs, string username);
    }
}
