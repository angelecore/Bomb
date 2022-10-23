using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public class LogsCommand : ICommand
    {
        GameState gameState;
        List<string> logs;

        public LogsCommand(GameState gameState, List<string> logs)
        {
            this.gameState = gameState;
            this.logs = logs;
        }

        public void execute()
        {
            using (
                FileStream fs = File.Create(
                    string.Format("{0}-{1}-logs.txt", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"), gameState.PlayerName)
                )
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
    }
}
