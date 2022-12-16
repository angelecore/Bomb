using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bomberman.classes.mediator;
using static ServiceStack.Diagnostics.Events;

namespace bomberman.classes.Compositetree
{
    //sita implementuoti kaip leaf
    public class ClusterBomb : Bomb, Component
    {
        public bool notExploded { get; set; }

        public ClusterBomb(Vector2f position, Player owner, int radius, string id, IGameManager gameManager, int generation = 0) : 
            base(position, owner, radius, generation, id, gameManager)
        {
            notExploded = true;
        }

        public object Clone(Vector2f position)
        {
            ClusterBomb temp = new ClusterBomb(position, this.Owner, this.Radius, Guid.NewGuid().ToString(), _gameManager);
            temp.Timer = 4f;
            return temp;
        }

        public new List<Tuple<Vector2f, int>> GetExplosionPositions(Block[,] grid, Func<Vector2f, bool> isPositionValid)
        {
            if (notExploded)
            {
                var positions = new List<Tuple<Vector2f, int>>();

                var directions = new List<Directions>() { Directions.Up, Directions.Down, Directions.Left, Directions.Right };
                for (int i = 0; i < 2; i++)
                {
                    for (int j = directions.Count - 1; j >= 0; j--)
                    {
                        var dir = directions[j];
                        var vector = Utils.MultiplyVector(Utils.GetDirectionVector(dir), i);
                        var newPos = Utils.AddVectors(Position, vector);

                        // If the position is not valid or the block is indestructable - stop
                        if (!isPositionValid(newPos) || grid[newPos.Y, newPos.X].Type == BlockType.InDestructable)
                        {
                            directions.RemoveAt(j);
                            continue;
                        }
                        if (positions.Contains(new Tuple<Vector2f, int>(newPos, (Radius - i) * 5)))
                            continue;
                        positions.Add(new Tuple<Vector2f, int>(newPos, (2 - i) * 5));
                    }
                }

                return positions;
            }
            else return new List<Tuple<Vector2f, int>>();
            

        }

        public List<Tuple<float, Component>> updatetimer(float miliSeconds)
        {
            if (notExploded)
            {
                this.Timer -= (float)miliSeconds * 0.001f;
                var temp = new List<Tuple<float, Component>>();
                temp.Add(new Tuple<float, Component>(this.Timer, this));
                return temp;
            }
            else return new List<Tuple<float, Component>>();

        }

        public List<Tuple<float, Component>> Gettimer()
        {
            if (notExploded)
            {
                var temp = new List<Tuple<float, Component>>();
                temp.Add(new Tuple<float, Component>(this.Timer, this));
                return temp;
            }
            else return new List<Tuple<float, Component>>();
        }

        public List<Component> GetBombs()
        {
                var temp = new List<Component>();
                temp.Add(this);
                return temp;
        }

        public List<Component> GetExplodedBombs()
        {
            var temp = new List<Component>();
            if(!notExploded)
                temp.Add(this);
            return temp;
        }
    }
}