namespace bomberman.classes
{
    public class Player
    {
        public PictureBox PlayerSprite { get; set; }
        public int CurrentCollum { get; set; }
        public int CurrentRow { get; set; }
        public Player(PictureBox sprite, int collum, int row)
        {
            PlayerSprite = sprite;
            CurrentCollum = collum;
            CurrentRow = row;
        }
    }
}
