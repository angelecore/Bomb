using bomberman.classes;
using System.Diagnostics;
using WebSocketSharp;

namespace bomberman
{
    public partial class Form1 : Form
    {
        private WebSocket? ws;

        const int blocksize = 50;
        private Button[,] blockmap;

        // key is the player id
        private Dictionary<string, PictureBox> playerSprites = new Dictionary<string, PictureBox>();
        // key is the bomb id
        private Dictionary<string, Tuple<PictureBox, Label>> bombSprites = new Dictionary<string, Tuple<PictureBox, Label>>();

        GameState gameState;
        private Stopwatch stopwatch = new Stopwatch();
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

        public void UpdateMap()
        {
            for (int y = 0; y < gameState.Grid.GetLength(0); y++)
            {
                for (int x = 0; x < gameState.Grid.GetLength(1); x++)
                {
                    var cell = gameState.Grid[y, x];
                    var block = blockmap[y, x];
                    if (gameState.ExplosionIntensity[y, x] > 0)
                        gameState.ExplosionIntensity[y, x]--;

                    switch (cell.Type)
                    {
                        case BlockType.InDestructable: // indestructable
                            block.BackColor = Color.Black;
                            break;
                        case BlockType.Destructable: // destructable
                            block.BackColor = Color.DarkGray;
                            break;
                        case BlockType.Empty:
                            block.BackColor = Color.LightGray;
                            break;
                    }

                    if (gameState.ExplosionIntensity[y, x] > 0)
                    { 
                        block.BackColor = Color.FromArgb(255 - 40 * gameState.ExplosionIntensity[y, x], 255, 192, 0);
                    }
                }
            }
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
                case "Bomb":
                    HandleBombPlacement(value);
                    break;
            }
        }
       
        private void HandleBombPlacement(string id)
        {
            var bomb = gameState.PlaceBomb(id);
            if (bomb == null) return;

            PictureBox bombImg = new PictureBox();
            bombImg.Image = Properties.Resources.bombSprite;
            bombImg.Size = new Size(blocksize, blocksize);
            bombImg.SizeMode = PictureBoxSizeMode.Zoom;
            bombImg.BackColor = Color.Transparent;
            bombImg.Location = new Point(bomb.Position.X * blocksize, bomb.Position.Y * blocksize);
            this.Controls.Add(bombImg);
            bombImg.BringToFront();


            Label timer = new Label();
            timer.Location = bombImg.Location;
            timer.Size = new Size(15, 15);
            timer.Text = ((int)bomb.Timer).ToString(); 
            timer.Parent = bombImg;
            timer.BackColor = Color.Transparent;

            this.Controls.Add(timer);
            timer.BringToFront();
            bombSprites[bomb.Id] = new Tuple<PictureBox, Label>(bombImg, timer);
            playerSprites[id].BringToFront();
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
            PlayerImg.Parent = blockmap[1, 1];
            PlayerImg.SizeMode = PictureBoxSizeMode.Zoom;
            PlayerImg.BackColor = Color.Transparent;
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
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            stopwatch.Restart();

            var explodedBombs = gameState.UpdateBombTimers(ts.TotalMilliseconds);
            foreach (var explodedBomb in explodedBombs)
            {
                this.Controls.Remove(bombSprites[explodedBomb.Id].Item1);
                this.Controls.Remove(bombSprites[explodedBomb.Id].Item2);
                bombSprites.Remove(explodedBomb.Id);
            }

            foreach (var bomb in gameState.Bombs)
            {
                bombSprites[bomb.Id].Item2.Text = ((int)bomb.Timer).ToString();
            }

            UpdateMap();
                
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
                    SendMoveCommandToServer("Right");
                    break;
                case Keys.Left:
                    SendMoveCommandToServer("Left");
                    break;
                case Keys.Down:
                    SendMoveCommandToServer("Down");
                    break;
                case Keys.Up:
                    SendMoveCommandToServer("Up");
                    break;
                case Keys.Enter:
                    SendBombCommandToServer();
                    break;
            }
        }

        private void SendMoveCommandToServer(string message)
        {
            if (ws != null && ws.ReadyState != WebSocketState.Closed)
            {
                ws.Send(string.Format("Move {0} {1}", gameState.PlayerId, message));
            }
        }
        private void SendBombCommandToServer()
        {
            if (ws != null && ws.ReadyState != WebSocketState.Closed)
            {
                ws.Send(string.Format("Bomb {0}", gameState.PlayerId));
            }
        }

        private void frm_menu_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
        
    }
}