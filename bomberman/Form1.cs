using bomberman.classes;
using WebSocketSharp;

namespace bomberman
{
    public partial class Form1 : Form
    {
        private WebSocket? ws;

        const int blocksize = 50;
        private Button[,] blockmap;
        private Dictionary<string, PictureBox> playerSprites = new Dictionary<string, PictureBox>();

        GameState gameState;
        public Form1(string name)
        {
            InitializeComponent();

            gameState = new GameState(name);
            DrawMap();
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

        public void DrawMap()
        {
            // Create map
            if (blockmap == null)
            {
                blockmap = new Button[gameState.Grid.GetLength(0), gameState.Grid.GetLength(1)];

                int currentx = 0;
                int currenty = 0;

                for (int y = 0; y < gameState.Grid.GetLength(0); y++)
                {
                    for (int x = 0; x < gameState.Grid.GetLength(1); x++)
                    {
                        var block = gameState.Grid[y, x];

                        Button button = new Button();
                        BlockType type = BlockType.Empty;

                        button.Size = new Size(blocksize, blocksize);
                       
                        switch (block.Type)
                        {
                            case BlockType.InDestructable: // indestructable
                                button.BackColor = Color.Black;
                                break;
                            case BlockType.Destructable: // destructable
                                button.BackColor = Color.DarkGray;
                                break;
                            case BlockType.Empty:
                                button.BackColor = Color.LightGray;
                                break;
                        }

                        button.Location = new Point(currentx, currenty);
                        this.Controls.Add(button);
                        this.blockmap[y, x] = button;

                        currentx += blocksize;
                    }
                    currentx = 0;
                    currenty += blocksize;
                }
            }
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
                    gameState.PerformAction(split[0], split[1]);
                    break;
            }
        }
       
        
        private void HandlePlayerJoined(string id)
        {
            var pos = gameState.AddEnemy(id);
            if (pos == null)
            {
                return;
            }

            Console.WriteLine("New player Joined => Session owner: {0} | Joiner id: {1}", gameState.PlayerName, id); 

            PictureBox PlayerImg = new PictureBox();
            PlayerImg.Image = Properties.Resources.character_positioned;
            PlayerImg.Size = new Size(blocksize, blocksize);
            PlayerImg.SizeMode = PictureBoxSizeMode.Zoom;
            PlayerImg.Location = new Point(pos.X * blocksize, pos.Y * blocksize);
            this.Controls.Add(PlayerImg);
            PlayerImg.BringToFront();

            playerSprites[id] = PlayerImg;
        }

        private void HandlePlayerConnected(string id)
        {
            Console.WriteLine("Owner connected => Name: {0} | Id: {1}", gameState.PlayerName, id);

            var pos = gameState.AddOwner(id);

            PictureBox PlayerImg = new PictureBox();
            PlayerImg.Image = Properties.Resources.character_positioned;
            PlayerImg.Size = new Size(blocksize, blocksize);
            PlayerImg.SizeMode = PictureBoxSizeMode.Zoom;
            Console.WriteLine("{0} {1}", pos.X, pos.Y);
            PlayerImg.Location = new Point(pos.X * blocksize, pos.Y * blocksize);
            this.Controls.Add(PlayerImg);
            PlayerImg.BringToFront();

            playerSprites[id] = PlayerImg;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void MovementTimer_Tick(object sender, EventArgs e)
        {
            var movingPlayers = gameState.GetMovingPlayers();
            foreach (var player in movingPlayers)
            {
                int speed = 5;
                int X = player.Position.X;
                int Y = player.Position.Y;

                var sprite = playerSprites[player.Id];

                switch (player.Direction)
                {
                    case Directions.Right:
                        sprite.Location = new Point(sprite.Location.X + speed, sprite.Location.Y);
                        if (blockmap[Y, X + 1].Location == sprite.Location)
                        {
                            player.Move();
                        }
                        break;
                    case Directions.Left:
                        sprite.Location = new Point(sprite.Location.X - speed, sprite.Location.Y);
                        if (blockmap[Y, X - 1].Location == sprite.Location)
                        {
                            player.Move();
                        }
                        break;
                    case Directions.Up:
                        sprite.Location = new Point(sprite.Location.X, sprite.Location.Y - speed);
                        if (blockmap[Y - 1, X].Location == sprite.Location)
                        {
                            player.Move();
                        }
                        break;
                    case Directions.Down:
                        sprite.Location = new Point(sprite.Location.X, sprite.Location.Y + speed);
                        if (blockmap[Y + 1, X].Location == sprite.Location)
                        {
                            player.Move();
                        }
                        break;
                }
                
            }
        }
        

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                    SendMessageToServer("Right");
                    break;
                case Keys.Left:
                    SendMessageToServer("Left");
                    break;
                case Keys.Down:
                    SendMessageToServer("Down");
                    break;
                case Keys.Up:
                    SendMessageToServer("Up");
                    break;
            }
        }

        private void SendMessageToServer(string message)
        {
            if (ws != null && ws.ReadyState != WebSocketState.Closed)
            {
                ws.Send(string.Format("Move {0} {1}", gameState.PlayerId, message));
            }
        }
        private void frm_menu_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
        
    }
}