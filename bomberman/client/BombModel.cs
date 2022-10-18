using bomberman.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.client
{
    internal class BombModel
    {
        public Point Position { get; set; }

        public Image BombSprite { get; set; }
        public PictureBox BombPictureBox { get; set; }
        public Label TimerLabel { get; set; }

        public BombModel(int time, Point position, Image sprite)
        {
            Position = position;
            BombSprite = sprite;

            BombPictureBox = new PictureBox();
            BombPictureBox.Size = new Size(Constants.BLOCK_SIZE, Constants.BLOCK_SIZE);
            BombPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            BombPictureBox.BackColor = Color.Transparent;
            BombPictureBox.Image = sprite;
            BombPictureBox.Location = position;

            TimerLabel = new Label();
            TimerLabel.Location = position;
            TimerLabel.Size = new Size(15, 15);
            TimerLabel.Text = time.ToString();
            TimerLabel.Parent = BombPictureBox;
            TimerLabel.BackColor = Color.Transparent;
        }

        public void BringToFront()
        {
            BombPictureBox.BringToFront();
            TimerLabel.BringToFront();
        }

        public Control[] GetControls()
        {
            return new Control[] { BombPictureBox, TimerLabel };
        }

        public void UpdatePosition(Point p)
        {
            Position = p;
            BombPictureBox.Location = Position;
            TimerLabel.Location = Position;
        }

        public void UpdateTimer(int time)
        {
            TimerLabel.Text = time.ToString();
        }
    }
}
