using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.adapter
{
    public class LogsPlainTextAdapter : ILogger
    {

        LogsPlainText adaptee = new LogsPlainText();

        public void output(List<string> logs, string username)
        {
            adaptee.outputPlain(logs, username);
        }
    }
}
