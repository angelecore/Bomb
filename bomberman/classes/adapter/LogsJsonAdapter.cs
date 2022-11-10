using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.adapter
{
    public class LogsJsonAdapter : ILogger
    {
        LogsJson adaptee = new LogsJson();
        public void output(List<string> logs, string username)
        {
            adaptee.output(logs, username);
        }
    }
}
