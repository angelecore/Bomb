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
        public PictureBox PlayerSprite { get; set; }
        public int CurrentCollum { get; set; }
        public int CurrentRow { get; set; }
        public Directions Direction { get; set; }

        public Player(string id, PictureBox sprite, int collum, int row)
        {
            this.Id = id;
            this.PlayerSprite = sprite;
            this.CurrentCollum = collum;
            this.CurrentRow = row;
            this.Direction = Directions.Idle;
        }

        public void SetDirection(Directions direction)
        {
            this.Direction = direction;
        }

    }
}
