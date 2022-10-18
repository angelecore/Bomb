using bomberman.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.client
{
    internal class PowerupModel : IModel
    {
        public Point Position { get; set; }

        public Image PowerupSprite { get; set; }
        public PictureBox PowerupPictureBox { get; set; }

        public PowerupModel(Point position, Image sprite)
        {
            Position = position;
            PowerupSprite = sprite;

            PowerupPictureBox = new PictureBox();
            PowerupPictureBox.Size = new Size(Constants.BLOCK_SIZE, Constants.BLOCK_SIZE);
            PowerupPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            PowerupPictureBox.BackColor = Color.Transparent;
            PowerupPictureBox.Image = sprite;
            PowerupPictureBox.Location = position;
        }

        public void BringToFront()
        {
            PowerupPictureBox.BringToFront();
        }

        public Control[] GetControls()
        {
            return new Control[] { PowerupPictureBox };
        }

        public void UpdatePosition(Point p)
        {
            Position = p;
            PowerupPictureBox.Location = Position;
        }
    }
}
