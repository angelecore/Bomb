using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public class CommandResolver
    {
        ICommand ?command;
        bool isActivate;
        public CommandResolver()
        {
            command = null;
            isActivate = true;
        }
        public void setCommand(ICommand command, bool isActivate)
        {
            this.command = command;
            this.isActivate = isActivate;
        }

        public void clearCommand()
        {
            this.command = null;
            this.isActivate = true;
        }

        public void activate()
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
