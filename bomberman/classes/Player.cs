using bomberman.classes.memento;

namespace bomberman.classes
{
    public enum Directions
    {
        Right,
        Left,
        Up,
        Down,
        Idle,
    }

    public enum BombType
    {
        Basic,
        Dynamite,
        Fire,
        Cluster
    }

    public class Player
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Vector2f Position { get; set; }
        public Directions Direction { get; set; }
        public bool IsAlive { get; set; }
        public int BombExplosionRadius { get; set; }
        public int Score { get; set; }
        public BombType BombType { get; set; }
        public int PlayerSpeed { get; set; }
        public List<PlayerTemporaryStats> TemporaryStats { get; set; }
        public Player(string id, string name, Vector2f position)
        {
            this.Id = id;
            this.Name = name;
            this.Position = position;
            this.Direction = Directions.Idle;
            this.IsAlive = true;
            this.BombExplosionRadius = 2;
            this.Score = 0;
            this.PlayerSpeed = 1;
            this.BombType = BombType.Cluster;
            TemporaryStats = new List<PlayerTemporaryStats>();
        }

        public PlayerSnapshot SnapshotPlayerInfo()
        {
            return new PlayerSnapshot(
                Position, 
                Direction, 
                BombExplosionRadius, 
                Score, 
                BombType, 
                PlayerSpeed, 
                TemporaryStats
           );
        }

        public void ApplySnapshot(PlayerSnapshot snapshot)
        {
            Position = snapshot.Position;
            Direction = snapshot.Direction;
            BombExplosionRadius = snapshot.BombExplosionRadius;
            Score = snapshot.Score;
            BombType = snapshot.BombType;
            PlayerSpeed = snapshot.PlayerSpeed;
            TemporaryStats = snapshot.TemporaryStats;
        }

        public void AddNewStat(PlayerTemporaryStats stat)
        {
            TemporaryStats.Add(stat);
        }

        public void UpdateTemporaryStats(float miliSecondsPassed)
        {
            for(int i = TemporaryStats.Count - 1; i >= 0; i--)
            {
                var stats = TemporaryStats[i];
                if (!stats.Applied)
                {
                    this.PlayerSpeed += stats.AddSpeedAmount ?? 0;
                    if (this.PlayerSpeed < 0)
                    {
                        this.PlayerSpeed = 0;
                    }
                    this.BombExplosionRadius += stats.AddRadiusAmount ?? 0;
                    stats.Applied = true;
                }

                stats.ActiveTimer -= miliSecondsPassed * 0.001f;


                // Effect finished
                if (stats.ActiveTimer != null && stats.ActiveTimer < 1.0)
                {
                    this.PlayerSpeed -= stats.AddSpeedAmount ?? 0;
                    if (this.PlayerSpeed > 1)
                    {
                        this.PlayerSpeed = 1;
                    }

                    this.BombExplosionRadius -= stats.AddRadiusAmount ?? 0;
                    TemporaryStats.RemoveAt(i);
                }
            }
        }

        public void Move()
        {
            Position = Utils.AddVectors(
                Position, 
                Utils.GetDirectionVector(Direction)
            );
            Direction = Directions.Idle;
        }

        public void SetDirection(Directions direction)
        {
            this.Direction = direction;
        }
    }
}
