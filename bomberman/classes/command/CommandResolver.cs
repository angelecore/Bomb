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
        public CommandResolver()
        {
            command = null;
        }
        public void setCommand(ICommand command)
        {
            this.command = command;
        }

        public void activate()
        {
            if (command != null)
            {
                command.execute();
            }
        }
    }
}
