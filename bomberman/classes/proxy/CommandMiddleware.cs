using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.proxy
{
    internal class CommandMiddleware : ICommandMiddleware
    {
        CommandResolver _commandResolver;
        public CommandMiddleware()
        {
            _commandResolver = new CommandResolver();
        }

        void ICommandMiddleware.Activate()
        {
            // Don't do anything if waiting for players
            if (GameDataSingleton.GetInstance().CurrentGameStatus == GameStatus.WaitingForPlayers)
            {
                return;
            }
            _commandResolver.Activate();
        }

        void ICommandMiddleware.ClearCommand()
        {
            _commandResolver.ClearCommand();
        }

        void ICommandMiddleware.SetCommand(ICommand command, bool isActivate)
        {
            _commandResolver.SetCommand(command, isActivate);
        }
    }
}
