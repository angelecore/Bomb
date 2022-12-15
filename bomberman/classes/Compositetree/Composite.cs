using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bomberman.classes.Compositetree
{
    // si klase veikia kaip sarasas
    // kitaip sakant mother cluster bomb
    public class Composite : Component // padaryti kaip cluster bomba buvo pries strategy tesiog pritaikyt composite kad nereiktu visu det i viena sarasa
    {
        private List<Component> BombList = new List<Component>();
        public void AddBomb(Component ele)
        {
            BombList.Add(ele);  
        }

        public void RemoveBomb(Component ele)
        {
            BombList.Remove(ele);
        }
        public List<Component> GetBranches()
        {
            return BombList;
        }

        public List<Tuple<Vector2f, int, Component>> GetExplosionPositions(Block[,] grid, Func<Vector2f, bool> isPositionValid)
        {
            var list = new List<Tuple<Vector2f, int, Component>>();
            foreach (Component bomb in BombList)
            {
                var templist = bomb.GetExplosionPositions(grid, isPositionValid);
                foreach(var item in templist )
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public object Clone(Vector2f position)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<float, Component>> updatetimer(float miliSeconds)
        {
            List<Tuple<float, Component>> list = new List<Tuple<float, Component>>();
            foreach (Component bomb in BombList)
            {
                var temp = bomb.updatetimer(miliSeconds);
                foreach (Tuple<float, Component> item in temp)
                    list.Add(item);
            }
            return list;
        }

        public List<Tuple<float, Component>> Gettimer()
        {
            List<Tuple<float, Component>> list = new List<Tuple<float, Component>>();
            foreach (Component bomb in BombList)
            {
                var temp = bomb.Gettimer();
                foreach (Tuple<float, Component> item in temp)
                    list.Add(item);
            }
            return list;
        }

        public List<Component> GetBombs()
        {
            List<Component> list = new List<Component>();
            foreach (Component bomb in BombList)
            {
                var temp = bomb.GetBombs();
                foreach (Component item in temp)
                    list.Add(item);
            }
            return list;
        }

        public List<Component> GetExplodedBombs()
        {
            List<Component> list = new List<Component>();
            foreach (Component bomb in BombList)
            {
                var temp = bomb.GetExplodedBombs();
                foreach (Component item in temp)
                    list.Add(item);
            }
            return list;
        }

    }
}
