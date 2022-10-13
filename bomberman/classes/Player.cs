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

    public class Player
    {
        public string Id { get; set; }
        public Vector2f Position { get; set; }
        public Directions Direction { get; set; }

        public bool IsAlive { get; set; }

        public Player(string id, Vector2f position)
        {
            this.Id = id;
            this.Position = position;
            this.Direction = Directions.Idle;
            this.IsAlive = true;
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
