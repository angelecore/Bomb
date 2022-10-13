using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    internal class Bomb
    {
        public string Id { get; set; }
        public Vector2f Position { get; set; }
        public float Timer { get; set; }
        public int Radius { get; set; }

        public Bomb(Vector2f position)
        {
            Position = position;
            Timer = 5.0f;
            Id = Guid.NewGuid().ToString();
            Radius = 4;
        }
    }
}
