using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public class LogsCommand : ICommand
    {
        List<string> logs;
        string filePath;

        public LogsCommand(List<string> logs, string filePath)
        {
            this.logs = logs;
            this.filePath = filePath;
        }

        public void execute()
        {
            using (
                FileStream fs = File.Create(filePath)
            ) {
                using (var fw = new StreamWriter(fs))
                {
                    foreach (var log in logs)
                    {
                        fw.WriteLine(log);
                        fw.Flush();
                    }
                }
            }
        }

        public void undo()
        {
            File.Delete(filePath);
        }
    }
}
