using bomberman.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.client
{
    internal class PlayerModel : IModel
    {
        public string Name { get; set; }

        public Point Position { get; set; }

        public Image PlayerSprite { get; set; }
        public PictureBox PlayerPictureBox { get; set; }
        public Label PlayerNameLabel { get; set; }

        public PlayerModel(string name, Point position, Image sprite)
        {
            Name = name;
            Position = position;
            PlayerSprite = sprite;

            PlayerPictureBox = new PictureBox();
            PlayerPictureBox.Size = new Size(Constants.BLOCK_SIZE, Constants.BLOCK_SIZE);
            PlayerPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            PlayerPictureBox.BackColor = Color.Transparent;
            PlayerPictureBox.Image = sprite;
            PlayerPictureBox.Location = position;

            PlayerNameLabel = new Label();
            PlayerNameLabel.Location = position;
            PlayerNameLabel.Size = new Size(Constants.BLOCK_SIZE, 8);
            PlayerNameLabel.Font = new Font("Arial", 5);
            PlayerNameLabel.Text = name;
            PlayerNameLabel.Parent = PlayerPictureBox;
            PlayerNameLabel.BackColor = Color.Transparent;
        }

        public void BringToFront()
        {
            PlayerPictureBox.BringToFront();
            PlayerNameLabel.BringToFront();
        }

        public Control[] GetControls()
        {
            return new Control[] { PlayerPictureBox, PlayerNameLabel };
        }

        public void UpdatePosition(Point p)
        {
            Position = p;
            PlayerPictureBox.Location = Position;
            PlayerNameLabel.Location = Position;
        }
    }
}
