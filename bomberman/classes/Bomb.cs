using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public abstract class Bomb
    {
        public string Id { get; set; }
        public Vector2f Position { get; set; }
        public float Timer { get; set; }
        public int Radius { get; set; }
        public Player Owner { get; set; }
        public int Generation { get; set; }

        public Bomb(Vector2f position, Player owner, int radius, int generation)
        {
            Position = position;
            Timer = 8.0f;
            Owner = owner;
            Id = Guid.NewGuid().ToString();
            Radius = radius;
            this.Generation = generation;
        }

        // Received grid and a lambda function that returns if given position is valid
        // Returns exploded positions with explosion distance attached to it
        public abstract List<Tuple<Vector2f, int>> GetExplosionPositions(Block[,] grid, Func<Vector2f, bool> isPositionValid);

        public abstract object Clone(Bomb bomb, Vector2f position, int generation);
    }
}
