using bomberman.classes;
using WebSocketSharp;

namespace bomberman
{
    public partial class Form1 : Form
    {
        private WebSocket? ws;
        private string PlayerName;
        private string PlayerId;

        const int blocksize = 50;

        private Block[,] blockmap = new Block[7, 7];

        private List<Tuple<int, int>> possiblePlayerPos = new List<Tuple<int, int>>();

        private List<Player> players = new List<Player>();

        public Form1(string name)
        {
            PlayerName = name;

            InitializeComponent();
            Initializemap(1);

            this.ws = new WebSocket("ws://127.0.0.1:7980/Laputa");
            this.MovementTimer.Enabled = true;
            ws.OnMessage += (sender, e) =>
            {
                if (!e.IsText) { return; }
                var data = e.Data.Split(" ", 2);
                var type = data[0];
                var value = data[1]; 
                HandleEvent(type, value);
            };

            ws.Connect();
            ws.Send(string.Format("Connected"));
        }

        public bool ControlInvokeRequired(Control c, Action a)
        {
            if (c.InvokeRequired) c.Invoke(new MethodInvoker(delegate { a(); }));
            else return false;

            return true;
        }

        private void HandleEvent(string type, string value)
        {
            // Super hacky, if you want to modify the form the thread needs to be the same so we need to reinvoke it
            if (ControlInvokeRequired(this, () => HandleEvent(type, value))) return;

            switch (type)
            {
                case "Connected":
                    HandlePlayerConnected(value);
                    break;
                case "Joined":
                    HandlePlayerJoined(value);
                    break;
                case "Move":
                    var split = value.Split(" ");
                    HandleMove(split[0], split[1]);
                    break;
            }
        }

        private void HandleMove(string id, string key)
        {
            var player = players.Find(p => p.Id == id);

            if (player == null) return;

            switch (key)
            {
                case "Up":
                    player.SetDirection(Directions.Up);
                    break;
                case "Down":
                    player.SetDirection(Directions.Down);
                    break;
                case "Left":
                    player.SetDirection(Directions.Left);
                    break;
                case "Right":
                    player.SetDirection(Directions.Right);
                    break;
            }
        }

        private void HandlePlayerJoined(string id)
        {
            // Hacky, but this is the owner id and it cannot join twice.
            // TODO: fix this somehow in the future :))))
            if (id == PlayerId)
            {
                return;
            }
            var pos = this.possiblePlayerPos[0];
            possiblePlayerPos.RemoveAt(0);
            Console.WriteLine("New player Joined => Session owner: {0} | Joiner id: {1}", PlayerName, id); 

            PictureBox PlayerImg = new PictureBox();
            PlayerImg.Image = Properties.Resources.character_positioned;
            PlayerImg.Size = new Size(blocksize, blocksize);
            PlayerImg.SizeMode = PictureBoxSizeMode.Zoom;
            PlayerImg.Location = blockmap[pos.Item1, pos.Item2].BlockObj.Location;
            this.Controls.Add(PlayerImg);
            PlayerImg.BringToFront();

            players.Add(new Player(id, PlayerImg, pos.Item1, pos.Item2));
        }
        private void HandlePlayerConnected(string id)
        {
            this.PlayerId = id;
            Console.WriteLine("Owner connected => Name: {0} | Id: {1}", PlayerName, id);

            var pos = this.possiblePlayerPos[0];
            possiblePlayerPos.RemoveAt(0);
 
            PictureBox PlayerImg = new PictureBox();
            PlayerImg.Image = Properties.Resources.character_positioned;
            PlayerImg.Size = new Size(blocksize, blocksize);
            PlayerImg.SizeMode = PictureBoxSizeMode.Zoom;
            PlayerImg.Location = blockmap[pos.Item1, pos.Item2].BlockObj.Location;
            this.Controls.Add(PlayerImg);
            PlayerImg.BringToFront();

            players.Add(new Player(id, PlayerImg, pos.Item1, pos.Item2));
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
            using (System.IO.StringReader reader = new System.IO.StringReader(maplayout))
            {

                int currentx = 0;
                int currenty = 0;
                string line = String.Empty;
      
                int currentRow = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    char[] chararray = line.ToArray(); 
                    int currentCollum = 0;
                    foreach (char charele in chararray)
                    {
                        Button block = new Button();
                        classes.BlockType type = classes.BlockType.Empty;
                        block.Size = new Size(blocksize, blocksize);
                        switch (charele)
                        {
                            case '#': // indestructable
                                block.BackColor = Color.Black;
                                type = classes.BlockType.InDestructable;
                                break;
                            case 'c': // destructable
                                block.BackColor = Color.DarkGray;
                                type = classes.BlockType.Destructable;
                                break;
                            case '.':
                                block.BackColor = Color.LightGray;
                                break;
                            default:
                                possiblePlayerPos.Add(new Tuple<int, int>(currentRow, currentCollum));
                                break;
                        }

                        block.Location = new Point(currentx, currenty);
                        this.Controls.Add(block);
                        currentx += blocksize;
                        this.blockmap[currentRow, currentCollum] = new Block(block, type);

                        currentCollum++;
                    }
                    currentRow++;
                    currentx = 0;
                    currenty += blocksize;

                }
                reader.Close();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void MovementTimer_Tick(object sender, EventArgs e)
        {
            foreach (var player in players)
            {
                if (player.Direction != Directions.Idle)
                {
                    int speed = 5;
                    int collum = player.CurrentCollum;
                    int row = player.CurrentRow;
                    switch (player.Direction)
                    {
                        case Directions.Right:
                            player.PlayerSprite.Location = new Point(player.PlayerSprite.Location.X + speed, player.PlayerSprite.Location.Y);
                            if (blockmap[row, collum + 1].BlockObj.Location == player.PlayerSprite.Location)
                            {
                                player.SetDirection(Directions.Idle);
                                player.CurrentCollum++;
                                //PlayerObject.CurrentRow++;
                            }
                            break;
                        case Directions.Left:
                            player.PlayerSprite.Location = new Point(player.PlayerSprite.Location.X - speed, player.PlayerSprite.Location.Y);
                            if (blockmap[row, collum - 1].BlockObj.Location == player.PlayerSprite.Location)
                            {
                                player.SetDirection(Directions.Idle);
                                player.CurrentCollum--;
                                //PlayerObject.CurrentRow++;

                            }
                            break;
                        case Directions.Up:
                            player.PlayerSprite.Location = new Point(player.PlayerSprite.Location.X, player.PlayerSprite.Location.Y - speed);
                            if (blockmap[row - 1, collum].BlockObj.Location == player.PlayerSprite.Location)
                            {
                                player.SetDirection(Directions.Idle);
                                //PlayerObject.CurrentCollum++;
                                player.CurrentRow--;
                            }
                            break;
                        case Directions.Down:
                            player.PlayerSprite.Location = new Point(player.PlayerSprite.Location.X, player.PlayerSprite.Location.Y + speed);
                            if (blockmap[row + 1, collum].BlockObj.Location == player.PlayerSprite.Location)
                            {
                                player.SetDirection(Directions.Idle);
                                //PlayerObject.CurrentCollum++;
                                player.CurrentRow++;
                            }
                            break;
                    }
                }
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            var player = players.Find(p => p.Id == PlayerId);
            int collum = player.CurrentCollum;
            int row = player.CurrentRow;
            switch (e.KeyCode)
            {

                case Keys.Right:
                    //int collum = PlayerObject.CurrentCollum;
                    //int row = PlayerObject.CurrentRow;
                    if (collum + 1 < blockmap.GetLength(1) && blockmap[row, collum + 1].Type == classes.BlockType.Empty)
                    {
                        SendMessageToServer("Right");
                    }
                    break;
                case Keys.Left:
                    //int collum = PlayerObject.CurrentCollum;
                    //int row = PlayerObject.CurrentRow;
                    if (collum - 1 > 0 && blockmap[row, collum - 1].Type == classes.BlockType.Empty)
                    {
                        SendMessageToServer("Left");
                    }
                    break;
                case Keys.Down:
                    //int collum = PlayerObject.CurrentCollum;
                    //int row = PlayerObject.CurrentRow;
                    if (row + 1 < blockmap.GetLength(0) && blockmap[row + 1, collum].Type == classes.BlockType.Empty)
                    {
                        SendMessageToServer("Down");
                    }
                    break;
                case Keys.Up:
                    //int collum = PlayerObject.CurrentCollum;
                    //int row = PlayerObject.CurrentRow;
                    if (row - 1 > 0 && blockmap[row - 1, collum].Type == classes.BlockType.Empty)
                    {
                        SendMessageToServer("Up");
                    }
                    break;

            }
        }

        private void SendMessageToServer(string message)
        {
            if (ws != null && ws.ReadyState != WebSocketState.Closed)
            {
                ws.Send(string.Format("Move {0} {1}", PlayerId, message));
            }
        }
        private void frm_menu_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

    }
}