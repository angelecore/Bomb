using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.flyweight
{
    public class FlyweightFactory
    {
        private List<Tuple<Flyweight, string>> flyweights = new List<Tuple<Flyweight, string>>();

        public FlyweightFactory(List<Tuple<Bitmap, string>> args = null)
        {
            foreach (var elem in args)
            {
                flyweights.Add(new Tuple<Flyweight, string>(new Flyweight(elem.Item1), elem.Item2));
            }
        }

        public Flyweight? addFlyweight(Bitmap sprite, string key)
        {
            flyweights.Add(new Tuple<Flyweight, string>(new Flyweight(sprite), key));

            return getFlyweight(key);
        }

        public Flyweight? getFlyweight(string key)
        {
            return flyweights.FirstOrDefault(x => x.Item2 == key).Item1;
        }
    }
}
