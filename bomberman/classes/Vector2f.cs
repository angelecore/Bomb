using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public class Vector2f
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Vector2f(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Vector2f)) return false;
            Vector2f other = (Vector2f)obj;
            return X == other.X && Y == other.Y;
        }

        public override string ToString()
        {
            return String.Format("({0};{1})", X, Y);
        }
    }
}
