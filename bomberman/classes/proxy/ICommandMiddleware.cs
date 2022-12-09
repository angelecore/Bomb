using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.proxy
{
    public interface ICommandMiddleware
    {
        public void Activate();
        public void ClearCommand();
        public void SetCommand(ICommand command, bool isActivate);
    }
}
