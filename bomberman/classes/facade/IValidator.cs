using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.facade
{
    public interface IValidator
    {
        public bool IsValid(Player player, Vector2f dirVector);
    }
}
