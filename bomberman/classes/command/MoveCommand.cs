using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public class MoveCommand : ICommand
    {
        GameState gameState;
        string playerId;
        string direction;

        public MoveCommand(GameState gameState, string playerId, string direction)
        {
            this.gameState = gameState;
            this.playerId = playerId;
            this.direction = direction;
        }

        public void execute()
        {
            if (GameDataSingleton.GetInstance().CurrentGameStatus == GameStatus.WaitingForPlayers)
            {
                return;
            }

            gameState.PerformAction(playerId, direction);
        }

        public void undo()
        {
            return;
        }
    }
}
