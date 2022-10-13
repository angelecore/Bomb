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

        private Label TopLabel;

        private Dictionary<BlockType, Color> BlockColors = new Dictionary<BlockType, Color>() 
        {
            { BlockType.Empty, Color.LightGray },
            { BlockType.Destructable, Color.DarkGray },
            { BlockType.InDestructable, Color.Black },
        };

        GameState gameState;
        private Stopwatch stopwatch = new Stopwatch();
        public Form1(string name)
        {
            InitializeComponent();

            gameState = new GameState(name);

            CreateMap();

            this.ws = new WebSocket("ws://127.0.0.1:7980/Laputa");
            this.MovementTimer.Enabled = true;

            // Handle socket messages
            ws.OnMessage += (sender, e) =>
            {
                if (!e.IsText) { return; }
                var data = e.Data.Split(" ", 2);
                var type = data[0];
                var value = data[1];
                HandleEvents(type, value);
            };

            ws.Connect();

            // Inform other users that you have connected
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

                    block.BackColor = BlockColors[cell.Type];

                    if (gameState.ExplosionIntensity[y, x] > 0)
                    {
                        gameState.ExplosionIntensity[y, x]--;
                        block.BackColor = Color.FromArgb(12 * gameState.ExplosionIntensity[y, x], 255, 192, 0);
                    }
                }
            }
        }

        public void CreateMap()
        {
            // Create map
            if (blockmap == null)
            {
                blockmap = new Button[gameState.Grid.GetLength(0), gameState.Grid.GetLength(1)];

                for (int y = 0; y < gameState.Grid.GetLength(0); y++)
                {
                    for (int x = 0; x < gameState.Grid.GetLength(1); x++)
                    {
                        var block = gameState.Grid[y, x];

                        Button button = new Button();
                        button.Size = new Size(blocksize, blocksize);
                        button.BackColor = BlockColors[block.Type];
                        button.Location = new Point(x * blocksize, y * blocksize);

                        this.Controls.Add(button);

                        this.blockmap[y, x] = button;
                    }
                }
            }
        }

        public bool ControlInvokeRequired(Control c, Action a)
        {
            if (c.InvokeRequired) c.Invoke(new MethodInvoker(delegate { a(); }));
            else return false;

            return true;
        }

        
        private void HandleEvents(string type, string value)
        {
            // Super hacky, if you want to modify the form the thread needs to be the same so we need to reinvoke it
            if (ControlInvokeRequired(this, () => HandleEvents(type, value))) return;

            switch (type)
            {
                case "Connected":
                    HandlePlayerConnected(value);
                    break;
                case "Joined":
                    HandlePlayerJoined(value);
                    break;
                case "Move":
                    if (gameState.CheckGameStatus() == GameStatus.WaitingForPlayers)
                    {
                        return;
                    }
                    var split = value.Split(" ");
                    gameState.PerformAction(split[0], split[1]);
                    break;
                case "Bomb":
                    if (gameState.CheckGameStatus() == GameStatus.WaitingForPlayers)
                    {
                        return;
                    }
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
            CreatePlayerSprite(id, pos);
        }

        private void HandlePlayerConnected(string id)
        {
            Console.WriteLine("Owner connected => Name: {0} | Id: {1}", gameState.PlayerName, id);

            var pos = gameState.AddOwner(id);
            CreatePlayerSprite(id, pos);
        }

        private void CreatePlayerSprite(string playerId, Vector2f position)
        {
            PictureBox PlayerImg = new PictureBox();

            PlayerImg.Image = Properties.Resources.character_positioned;

            PlayerImg.Size = new Size(blocksize, blocksize);
            PlayerImg.Parent = blockmap[1, 1];
            PlayerImg.SizeMode = PictureBoxSizeMode.Zoom;
            PlayerImg.BackColor = Color.Transparent;
            PlayerImg.Location = new Point(position.X * blocksize, position.Y * blocksize);
            this.Controls.Add(PlayerImg);
            PlayerImg.BringToFront();

            playerSprites[playerId] = PlayerImg;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

        }


        private void MovementTimer_Tick(object sender, EventArgs e)
        {
            stopwatch.Stop();

            if(gameState.CheckGameStatus() == GameStatus.WaitingForPlayers)
            {
                if (TopLabel == null)
                {
                    TopLabel = new Label();
                    TopLabel.Size = new Size(300, 40);
                    TopLabel.Font = new Font("Calibri", 24);
                    TopLabel.Location = new Point(0, 0);   
                    this.Controls.Add(TopLabel);
                    TopLabel.Text = "Waiting for players...";
                    TopLabel.BringToFront();
                }
                return;
            } 
            else
            {
                this.Controls.Remove(TopLabel);
                TopLabel = null;
            }
            // Update bomb timer labels and remove exploded bombs
            UpdateBombs();
            // Update information on the map based on the new state
            UpdateMap();

            // Remove dead players
            RemoveDeadPlayers(gameState.GetKilledPlayers());

            // Move player sprites, whose state is not idle
            MoveAnimations();

            // Check if final game state has been reached
            var gameStatus = gameState.CheckGameStatus();

            // End the game
            if (this.Visible && (gameStatus == GameStatus.Won || gameStatus == GameStatus.Lost || gameStatus == GameStatus.Tie))
            {
                this.Hide();
                // Open final form
                string text = "";
                if (gameStatus == GameStatus.Won)
                {
                    text = String.Format("Congratulations {0}, You have Won!", gameState.PlayerName);
                } 
                else if (gameStatus == GameStatus.Lost)
                {
                    text = "You have lost...";
                } 
                else
                {
                    text = "It's a tie";
                }

                EndGameForm form = new EndGameForm(text);
                form.Show();
            }

            stopwatch.Restart();
        }
        
        private void RemoveDeadPlayers(List<Player> deadPlayers)
        {
            foreach(var player in deadPlayers)
            {
                this.Controls.Remove(playerSprites[player.Id]);
                playerSprites.Remove(player.Id);

                // Now we can delete this player from the game
                gameState.RemovePlayer(player.Id);
            }
        }

        private void UpdateBombs()
        {
            TimeSpan ts = stopwatch.Elapsed;
            // Remove exploded bombs sprites and labels
            var explodedBombs = gameState.UpdateBombTimers(ts.TotalMilliseconds);
            foreach (var explodedBomb in explodedBombs)
            {
                this.Controls.Remove(bombSprites[explodedBomb.Id].Item1);
                this.Controls.Remove(bombSprites[explodedBomb.Id].Item2);
                bombSprites.Remove(explodedBomb.Id);
            }

            // Update timer labels
            foreach (var bomb in gameState.Bombs)
            {
                bombSprites[bomb.Id].Item2.Text = ((int)bomb.Timer).ToString();
            }
        }

        private void MoveAnimations()
        {
            var movingPlayers = gameState.GetMovingPlayers();
            foreach (var player in movingPlayers)
            {
                int speed = 5;
                var sprite = playerSprites[player.Id];

                var dirVector = Utils.GetDirectionVector(player.Direction);
                var nextSpritePos = Utils.AddVectors(
                    new Vector2f(sprite.Location.X, sprite.Location.Y),
                    Utils.MultiplyVector(dirVector, speed)
                );
                var nextPlayerPos = Utils.AddVectors(player.Position, dirVector);

                sprite.Location = new Point(nextSpritePos.X, nextSpritePos.Y);

                if (blockmap[nextPlayerPos.Y, nextPlayerPos.X].Location == sprite.Location)
                {
                    player.Move();
                }
            }

        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                case Keys.D:
                    SendMoveCommandToServer("Right");
                    break;
                case Keys.Left:
                case Keys.A:
                    SendMoveCommandToServer("Left");
                    break;
                case Keys.Down:
                case Keys.S:
                    SendMoveCommandToServer("Down");
                    break;
                case Keys.Up:
                case Keys.W:
                    SendMoveCommandToServer("Up");
                    break;
                case Keys.Enter:
                case Keys.Space:
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