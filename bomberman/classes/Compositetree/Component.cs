using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.Compositetree
{
    // tie kurie implementuoja sita interface gali buti dedami i composite
    public interface Component
    {

        public List<Tuple<Vector2f, int>> GetExplosionPositions(Block[,] grid, Func<Vector2f, bool> isPositionValid);
        public object Clone(Vector2f position);

        public List<Tuple<float, Component>> updatetimer(float miliSeconds);
        public List<Tuple<float, Component>> Gettimer();

        public List<Component> GetBombs();
        public List<Component> GetExplodedBombs();

        //public List<Component> GetParent();
    }
}
