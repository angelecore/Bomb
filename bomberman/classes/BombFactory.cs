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
                    return new BasicBomb(position, owner, radius,0);
                case BombType.Dynamite:
                    return new DynamiteBomb(position, owner, radius,0);
                case BombType.Fire:
                    return new FireBomb(position, owner, radius,0);
                case BombType.Cluster:
                    return new ClusterBomb(position, owner, radius,0);
                default:
                    break;
            }
            return null;
        }
    }
}
