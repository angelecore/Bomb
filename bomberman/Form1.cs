using System.Windows.Forms;

namespace bomberman
{
    public partial class Form1 : Form
    {
        public enum Directions
        {
            Right,
            Left,
            Up,
            Down
        }
        private classes.Block[,] blockmap = new classes.Block[7,7];
        classes.Player PlayerObject;
        public Form1()
        {
            InitializeComponent();
            Initializemap(1);
        }

        private void Initializemap(int map)
        {
            
            if (map <= 0)
            {
                MessageBox.Show(String.Format("no map"));
                return;
            }

            string maplayout = string.Empty;
            switch (map)
            {
                case 1:
                    maplayout = Properties.Resources.Level1;
                    break;

            }
            int blocksize = 50;
            using (System.IO.StringReader reader = new System.IO.StringReader(maplayout))
            {
                
                int currentx = 0;
                int currenty = 0;
                string line = String.Empty;
                int currentCollum =0;
                int currentRow = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] chararray = line.Split(' ');
                    foreach (string charele in chararray)
                    { Button block = new Button();
                        classes.BlockType type = classes.BlockType.Empty;
                        block.Size = new Size(blocksize, blocksize);
                        switch(charele)
                        {
                            case "i": // indestructable
                                block.BackColor = Color.Black;
                                type = classes.BlockType.InDestructable;
                                break;
                            case "c": // destructable
                                block.BackColor = Color.DarkGray;
                                type = classes.BlockType.Destructable;
                                break;
                            case "v":
                                block.BackColor = Color.LightGray;
                                break;
                        }
                        block.Location = new Point(currentx, currenty);
                        this.Controls.Add(block);
                        currentx += blocksize;
                        this.blockmap[currentRow, currentCollum] = new classes.Block(block ,type);
                        currentCollum++;
                    }
                    currentRow++;
                    currentCollum = 0;
                    currentx = 0;
                    currenty += blocksize;
                    
                }
                reader.Close();
            }
            PictureBox PlayerImg = new PictureBox();
            PlayerImg.Image = Properties.Resources.character_positioned;
            PlayerImg.Size = new Size(blocksize, blocksize);
            PlayerImg.SizeMode = PictureBoxSizeMode.Zoom;
            PlayerImg.Location = blockmap[1, 1].BlockObj.Location;
            this.Controls.Add(PlayerImg);
            PlayerImg.BringToFront();
            PlayerObject = new classes.Player(PlayerImg, 1, 1);
        }
        bool move = false;
        Directions playerdirection = Directions.Down;

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void MovementTimer_Tick(object sender, EventArgs e)
        {
            if (move)
            {
                int speed = 5;
                int collum = PlayerObject.CurrentCollum;
                int row = PlayerObject.CurrentRow;
                switch (playerdirection)
                {
                    case Directions.Right:
                        PlayerObject.PlayerSprite.Location = new Point(PlayerObject.PlayerSprite.Location.X+ speed, PlayerObject.PlayerSprite.Location.Y);
                        if(blockmap[row,collum+1].BlockObj.Location == PlayerObject.PlayerSprite.Location)
                        {
                            move =false;
                            PlayerObject.CurrentCollum++;
                            //PlayerObject.CurrentRow++;
                            this.MovementTimer.Enabled = false;
                        }
                        break;
                    case Directions.Left:
                        PlayerObject.PlayerSprite.Location = new Point(PlayerObject.PlayerSprite.Location.X - speed, PlayerObject.PlayerSprite.Location.Y);
                        if (blockmap[row, collum - 1].BlockObj.Location == PlayerObject.PlayerSprite.Location)
                        {
                            move = false;
                            PlayerObject.CurrentCollum--;
                            //PlayerObject.CurrentRow++;
                            this.MovementTimer.Enabled = false;
                        }
                        break;
                    case Directions.Up:
                        PlayerObject.PlayerSprite.Location = new Point(PlayerObject.PlayerSprite.Location.X , PlayerObject.PlayerSprite.Location.Y - speed);
                        if (blockmap[row-1, collum].BlockObj.Location == PlayerObject.PlayerSprite.Location)
                        {
                            move = false;
                            //PlayerObject.CurrentCollum++;
                            PlayerObject.CurrentRow--;
                            this.MovementTimer.Enabled = false;
                        }
                        break;
                    case Directions.Down:
                        PlayerObject.PlayerSprite.Location = new Point(PlayerObject.PlayerSprite.Location.X, PlayerObject.PlayerSprite.Location.Y+ speed);
                        if (blockmap[row+1, collum].BlockObj.Location == PlayerObject.PlayerSprite.Location)
                        {
                            move = false;
                            //PlayerObject.CurrentCollum++;
                            PlayerObject.CurrentRow++;
                            this.MovementTimer.Enabled = false;
                        }
                        break;
                }
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            int collum = PlayerObject.CurrentCollum;
            int row = PlayerObject.CurrentRow;
            switch (e.KeyCode)
            {

                case Keys.Right:
                    //int collum = PlayerObject.CurrentCollum;
                    //int row = PlayerObject.CurrentRow;
                    if (collum + 1 < blockmap.GetLength(1) && blockmap[row, collum+1].Type == classes.BlockType.Empty)
                    {
                        move = true;
                        this.MovementTimer.Enabled = true;
                        playerdirection = Directions.Right;
                    }
                    break;
                case Keys.Left:
                    //int collum = PlayerObject.CurrentCollum;
                    //int row = PlayerObject.CurrentRow;
                    if (collum - 1 > 0 && blockmap[row, collum - 1].Type == classes.BlockType.Empty)
                    {
                        move = true;
                        this.MovementTimer.Enabled = true;
                        playerdirection = Directions.Left;
                    }
                    break;
                case Keys.Down:
                    //int collum = PlayerObject.CurrentCollum;
                    //int row = PlayerObject.CurrentRow;
                    if (row + 1 < blockmap.GetLength(0) && blockmap[row+1, collum].Type == classes.BlockType.Empty)
                    {
                        move = true;
                        this.MovementTimer.Enabled = true;
                        playerdirection = Directions.Down;
                    }
                    break;
                case Keys.Up:
                    //int collum = PlayerObject.CurrentCollum;
                    //int row = PlayerObject.CurrentRow;
                    if (row - 1 > 0 && blockmap[row-1, collum].Type == classes.BlockType.Empty)
                    {
                        move = true;
                        this.MovementTimer.Enabled = true;
                        playerdirection = Directions.Up;
                    }
                    break;

            }
        }
    }
}