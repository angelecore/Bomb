using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bomberman.classes.mediator;

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
        protected GameManager _gameManager;
        public Bomb(Vector2f position, Player owner, int radius, int generation, string id, GameManager gameManager)
        {
            Position = position;
            Timer = 8.0f;
            Owner = owner;
            Id = id;
            Radius = radius;
            this.Generation = generation;

            _gameManager = gameManager;
        }
        //(Block[,] grid, Func<Vector2f, bool> isPositionValid)

        public List<Tuple<Vector2f, int>> GetExplosionPositions(Block[,] grid, Func<Vector2f, bool> isPositionValid)
        {
            return ExplosionAlgoritham.ExplosionPossitions(grid, isPositionValid, this);
        }

        public void UpdateTimer(float miliSecondsPassed)
        {
            Timer -= (float)miliSecondsPassed * 0.001f;
            _gameManager.UpdateBombTimer(this);
            if (Timer < 1)
            {
                _gameManager.ExplodeBomb(this);
            }
        }

        public object Clone(Vector2f position, int generation)
        {
            Bomb temp = new Bomb(position, this.Owner, this.Radius, generation , Guid.NewGuid().ToString(), _gameManager);
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
