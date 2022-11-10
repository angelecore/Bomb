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
        public int Generation { get; set; }
        public IExplode ExplosionAlgoritham { get; set; }

        public Bomb(Vector2f position, Player owner, int radius, int generation)
        {
            Position = position;
            Timer = 8.0f;
            Owner = owner;
            Id = Guid.NewGuid().ToString();
            Radius = radius;
            this.Generation = generation;
        }
        //(Block[,] grid, Func<Vector2f, bool> isPositionValid)

        public List<Tuple<Vector2f, int>> GetExplosionPositions(Block[,] grid, Func<Vector2f, bool> isPositionValid)
        {
            return ExplosionAlgoritham.ExplosionPossitions(grid, isPositionValid, this);
        }
        public object Clone(Bomb bomb, Vector2f position, int generation)
        {
            Bomb temp = new Bomb(position, bomb.Owner, bomb.Radius, generation);
            temp.Timer = 2f;
            temp.ExplosionAlgoritham = this.ExplosionAlgoritham;
            return temp;
        }

        public void setExplosion(BombType type)
        {
            //this.ExplosionAlgoritham = explosion;
            switch (type)
            {
                case BombType.Basic:
                    {
                        this.ExplosionAlgoritham = new BasicExplosion();
                        return;
                    }
                case BombType.Dynamite:
                    {
                        this.ExplosionAlgoritham = new DynamiteExplosion();
                        return;
                    }
                case BombType.Fire:
                    {
                        this.ExplosionAlgoritham = new FireExplosion();
                        return;
                    }
                case BombType.Cluster:
                    {
                        this.ExplosionAlgoritham = new ClusterExplosion();
                        return;
                    }
                default:
                    break;
            }
        }

        }
}
