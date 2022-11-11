using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.adapter
{
    public class LogsPlainText
    {
        public void outputPlain(List<string> logs, string username)
        {
            using (
                FileStream fs = File.Create(
                    string.Format("{0}-{1}-logs.txt", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"), username)
                )
            )
            {
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
    }
}
