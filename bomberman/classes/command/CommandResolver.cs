using bomberman.classes.proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public class CommandResolver : ICommandMiddleware
    {
        ICommand ?command;
        bool isActivate;
        public CommandResolver()
        {
            command = null;
            isActivate = true;
        }
        public void SetCommand(ICommand command, bool isActivate)
        {
            this.command = command;
            this.isActivate = isActivate;
        }

        public void ClearCommand()
        {
            this.command = null;
            this.isActivate = true;
        }

        public void Activate()
        {
            if (command != null && isActivate)
            {
                command.execute();
            } 
            else if(command != null && !isActivate)
            {
                command.undo();
            }
        }
    }
}
