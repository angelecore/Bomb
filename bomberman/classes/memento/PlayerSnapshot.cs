using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.memento
{
    public class PlayerSnapshot
    {
        public Vector2f Position { get; set; }
        public Directions Direction { get; set; }
        public int BombExplosionRadius { get; set; }
        public int Score { get; set; }
        public BombType BombType { get; set; }
        public int PlayerSpeed { get; set; }
        public List<PlayerTemporaryStats> TemporaryStats { get; set; }

        public PlayerSnapshot(Vector2f position, Directions direction, int bombExplosionRadius, int score, BombType bombType, int playerSpeed, List<PlayerTemporaryStats> temporaryStats)
        {
            Position = position;
            Direction = direction;
            BombExplosionRadius = bombExplosionRadius;
            Score = score;
            BombType = bombType;
            PlayerSpeed = playerSpeed;
            // Clone variables
            TemporaryStats = temporaryStats
                .Select(stat => stat.DeepClone())
                .ToList();
        }
    }
}
