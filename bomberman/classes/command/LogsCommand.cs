using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bomberman.classes.adapter;
using Newtonsoft.Json;

namespace bomberman.classes
{
    public class LogsCommand : ICommand
    {
        GameState gameState;
        List<string> logs;
        ILogger adapter;


        public LogsCommand(GameState gameState, List<string> logs, ILogger adapter)
        {
            this.gameState = gameState;
            this.logs = logs;
            this.adapter = adapter;
        }

        public void execute()
        {
            adapter.output(logs, gameState.PlayerName);
        }
    }
}
