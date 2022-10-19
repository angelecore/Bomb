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
        public Player(string id, string name, Vector2f position)
        {
            this.Id = id;
            this.Name = name;
            this.Position = position;
            this.Direction = Directions.Idle;
            this.IsAlive = true;
            this.BombExplosionRadius = 2;
            this.Score = 0;
            this.BombType = BombType.Basic;
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
