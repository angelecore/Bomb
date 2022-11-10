using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.adapter
{
    public class LogsJson
    {
        public void output(List<string> logs, string username)
        {
            using (
                FileStream fs = File.Create(
                    string.Format("{0}-{1}-logs.json", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"), username)
                )
            )
            {
                using (var fw = new StreamWriter(fs))
                {
                    fw.WriteLine(JsonConvert.SerializeObject(logs));
                    fw.Flush();
                }
            }
        }
    }
}
