using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public class Bomb
    {
        public string Id { get; set; }
        public Vector2f Position { get; set; }
        public float Timer { get; set; }
        public int Radius { get; set; }
        public Player Owner { get; set; }

        public Bomb(Vector2f position, Player owner, int radius)
        {
            Position = position;
            Timer = 8.0f;
            Owner = owner;
            Id = Guid.NewGuid().ToString();
            Radius = radius;
        }
    }
}
