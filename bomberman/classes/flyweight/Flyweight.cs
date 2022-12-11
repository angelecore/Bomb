using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.flyweight
{
    public class Flyweight
    {
        private Bitmap image;

        public Flyweight(Bitmap image)
        {
            this.image = image;
        }

        public Bitmap getSprite()
        {
            return image;
        }
    }
}
