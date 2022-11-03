using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public static class BombFactory
    {
        public static Bomb GetBombInstance(BombType type, Vector2f position, Player owner, int radius)
        {
            switch (type)
            {
                case BombType.Basic:
                    return new BasicBomb(position, owner, radius);
                case BombType.Dynamite:
                    return new DynamiteBomb(position, owner, radius);
                case BombType.Fire:
                    return new FireBomb(position, owner, radius);
                default:
                    break;
            }
            return null;
        }
    }
}
