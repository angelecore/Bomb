using bomberman.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.client
{
    public class PlayerModel : IModel, IPlayerDecorator
    {
        public string Name { get; set; }

        public int PlacedBombs { get; set; }

        public Point Position { get; set; }

        public Image PlayerSprite { get; set; }
        public PictureBox PlayerPictureBox { get; set; }
        public Label PlayerNameLabel { get; set; }

        public Label PlayerBombsPlacedLabel { get; set; }

        public PlayerModel(string name, int placedBombs, Point position, Image sprite)
        {
            Name = name;
            Position = position;
            PlayerSprite = sprite;
            PlacedBombs = placedBombs;
        }

        public void BringToFront()
        {
            PlayerPictureBox.BringToFront();
            PlayerNameLabel.BringToFront();
            PlayerBombsPlacedLabel.BringToFront();
        }

        public Control[] GetControls()
        {
            return new Control[] { PlayerPictureBox, PlayerNameLabel, PlayerBombsPlacedLabel };
        }

        public void UpdatePosition(Point p)
        {
            Position = p;
            PlayerPictureBox.Location = Position;
            PlayerNameLabel.Location = Position;
            PlayerBombsPlacedLabel.Location = new Point(Position.X, Position.Y + 18); ;
        }

        public void UpdatePlayerBombsPlaced(int bombsCount)
        {
            PlacedBombs = bombsCount;
            PlayerBombsPlacedLabel.Text = PlacedBombs.ToString();
        }

        public IPlayerDecorator decorate()
        {
            return this;
        }

        public string getName()
        {
            return Name;
        }

        public int getPlacedBombs()
        {
            return PlacedBombs;
        }

        public Point getPosition()
        {
            return Position;
        }

        public Image getPlayerSprite()
        {
            return PlayerSprite;
        }
    }
}
