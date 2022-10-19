using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    internal class ChangeBombTypeStrategy : IBombtype
    {
        public BombType BombType { get; set; }
        public ChangeBombTypeStrategy(BombType type)
        {
            BombType = type;
        }
        public void ChangeBombType(Player player)
        {
            player.BombType = BombType;
        }
    }
}
