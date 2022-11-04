using bomberman.classes;
using bomberman.classes.decorator;
using bomberman.client;
using Newtonsoft.Json;
using System.Diagnostics;
using WebSocketSharp;

namespace bomberman
{
    public partial class Form1 : Form
    {
        private WebSocket? ws;

        private Button[,] blockmap;

        // key is the player id
        private Dictionary<string, PlayerModel> playerSprites = new Dictionary<string, PlayerModel>();
        // key is the bomb id
        private Dictionary<string, BombModel> bombSprites = new Dictionary<string, BombModel>();

        // key is the powerup grid index
        private Dictionary<int, PowerupModel> powerupSprites = new Dictionary<int, PowerupModel>();

        private List<InventoryTile> invetoryTiles = new List<InventoryTile>();

        private Label TopLabel;

        private Dictionary<BlockType, Color> BlockColors = new Dictionary<BlockType, Color>() 
        {
            { BlockType.Empty, Color.LightGray },
            { BlockType.Destructable, Color.DarkGray },
            { BlockType.InDestructable, Color.Black },
            { BlockType.Fire, Color.Red },
        };

        GameState gameState;

        private Stopwatch stopwatch = new Stopwatch();

        CommandResolver commandResolver;
        public Form1(string name)
        {
            InitializeComponent();

            gameState = new GameState(name);
            commandResolver = new CommandResolver();

            CreateMap();

            this.ws = new WebSocket("ws://127.0.0.1:7980/Server");
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
            ws.Send(string.Format("Connected {0}", name));

            CreateInventory();
        }

        private void CreateInventory()
        {
            var label = new Label();
            label.Location = new Point(blockmap.GetLength(1) * Constants.BLOCK_SIZE + 10, 10);
            label.Size = new Size(100, 20);
            label.Text = "Inventory";
            label.BackColor = Color.Transparent;
            this.Controls.Add(label);

  
        }

        private void UpdateInventory()
        {
            Player? ownerPlayer = gameState.GetOwnerPlayer();

            if (ownerPlayer == null)
            {
                return;
            }

            SetBaseInvetoryStats(new string[]
            {
                String.Format("Name: {0}", ownerPlayer?.Name),
                String.Format("Position: {0}", ownerPlayer?.Position),
                String.Format("Speed: {0}", ownerPlayer?.PlayerSpeed),
                String.Format("Moving Direction: {0}", ownerPlayer?.Direction),
                String.Format("Bomb radius: {0}", ownerPlayer?.BombExplosionRadius),
                String.Format("Bomb type: {0}", ownerPlayer?.BombType)
            });

            HashSet<string> updatedTilesIds = new HashSet<string>();
            // add effects at the end
            for(int i = invetoryTiles.Count - 1; i >= 0; i--)
            {
                var tile = invetoryTiles[i];
                if (tile.OwnershipId == null)
                {
                    continue;
                }

                var stat = ownerPlayer?.TemporaryStats?.SingleOrDefault(stat => stat.Id == tile.OwnershipId);
                if (stat == null)
                {
                    // remove this tile
                    RemoveControlsRange(tile.GetControls());
                    invetoryTiles.RemoveAt(i);
                    continue;
                }

                updatedTilesIds.Add(tile.OwnershipId);
                // update the tile text
                tile.UpdateText(stat.ToString());
            }

            foreach(var stat in ownerPlayer.TemporaryStats)
            {
                if (updatedTilesIds.Contains(stat.Id))
                {
                    continue;
                }

                // add new effect
                AddNewInvetoryTile(stat.ToString(), stat.Id);
            }
        }

        private void UpdateFire()
        {
            TimeSpan temp = stopwatch.Elapsed;
            List<Fire> FireRemove = new List<Fire>();
            List<FireController> ControllerRemove = new List<FireController>();
            foreach (Fire fire in gameState.FireList)
            {

                if (fire.Timer <= 0)
                {
                    foreach (FireController item in gameState.FireControllerList)
                    {
                        if (item.ID == fire.ID)
                        { 
                            gameState.Grid[item.BlockY, item.BlockX].Type = BlockType.Empty;
                            ControllerRemove.Add(item);
                        }

                    }
                    FireRemove.Add(fire);
                }
                fire.Timer -= (float)temp.TotalMilliseconds * 0.001f;
            }
            if (FireRemove.Count > 0)
            {
                foreach (Fire fire in FireRemove)
                    gameState.FireList.Remove(fire);
                foreach (FireController fire in ControllerRemove)
                    gameState.FireControllerList.Remove(fire);
            }
        }

        private void SetBaseInvetoryStats(string[] texts)
        {
            if (invetoryTiles.Count < texts.Length)
            {
                foreach(var text in texts)
                {
                    AddNewInvetoryTile(text);
                }
            } else
            {
                for (int i = 0; i < texts.Length; i++)
                {
                    invetoryTiles[i].UpdateText(texts[i]);
                }
            }
        }

        private void AddNewInvetoryTile(string text, string ?ownershipId = null)
        {
            int locationY = 30 + invetoryTiles.Count * 20 + 10;
            invetoryTiles.Add(
                new InventoryTile(
                    text,
                    new Point(blockmap.GetLength(1) * Constants.BLOCK_SIZE + 10, locationY),
                    ownershipId
                )
            );
            this.Controls.AddRange(invetoryTiles.Last().GetControls());
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

                    int gridIndex = gameState.GetGridIndex(cell.Position);
                    // add or delete power ups
                    if (gameState.Powerups.ContainsKey(gridIndex)) {
                        // If it doesn't exist - Create it
                        if (!powerupSprites.ContainsKey(gridIndex))
                        {
                            CreatePowerupModel(cell.Position, gameState.Powerups[gridIndex]);
                        }
                    } 

                    // add or delete BombChange
                    else if (gameState.Bombtypes.ContainsKey(gridIndex))
                    {
                        // If it doesn't exist - Create it
                        if (!powerupSprites.ContainsKey(gridIndex))
                        {
                            CreateDynamiteModel(cell.Position, gameState.Bombtypes[gridIndex]);
                        }
                    }
                    else if (powerupSprites.ContainsKey(gridIndex))
                    {
                        // delete this power up from the UI
                        RemoveControlsRange(powerupSprites[gridIndex].GetControls());
                        powerupSprites.Remove(gridIndex);
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
                        button.Size = new Size(Constants.BLOCK_SIZE, Constants.BLOCK_SIZE);
                        button.BackColor = BlockColors[block.Type];
                        button.Location = new Point(x * Constants.BLOCK_SIZE, y * Constants.BLOCK_SIZE);

                        this.Controls.Add(button);

                        this.blockmap[y, x] = button;
                    }
                }
            }
        }

        public void BringPlayerSpritesToFront()
        {
            foreach(var player in playerSprites)
            {
                player.Value.BringToFront();
            }
        }

        public void CreatePowerupModel(Vector2f position, IPowerup powerup)
        {
            Bitmap sprite = null;
            if (powerup is AddBombRadiusStrategy)
            {
                sprite = Properties.Resources.bombRadiusPowerup;
            } else if (powerup is SpeedPowerupStrategy)
            {
                sprite = Properties.Resources.speedPowerupIcon;
            }

            var powerupModel = new PowerupModel(
                new Point(position.X * Constants.BLOCK_SIZE, position.Y * Constants.BLOCK_SIZE),
                sprite
            );
            this.Controls.AddRange(powerupModel.GetControls());

            powerupModel.BringToFront();
            BringPlayerSpritesToFront();
            powerupSprites[gameState.GetGridIndex(position)] = powerupModel;
        }

        public void CreateDynamiteModel(Vector2f position, IBombtype dymamite)
        {
            var DynamiteModel = new PowerupModel(
                new Point(position.X * Constants.BLOCK_SIZE, position.Y * Constants.BLOCK_SIZE),
                Properties.Resources.dynamite
            );
            this.Controls.AddRange(DynamiteModel.GetControls());

            DynamiteModel.BringToFront();
            BringPlayerSpritesToFront();
            powerupSprites[gameState.GetGridIndex(position)] = DynamiteModel;
        }

        public bool ControlInvokeRequired(Control c, Action a)
        {
            if (c.InvokeRequired) c.Invoke(new MethodInvoker(delegate { a(); }));
            else return false;

            return true;
        }

        public void setBombSprites(Dictionary<string, BombModel> bombSprites)
        {
            this.bombSprites = bombSprites;
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
                    var split1 = value.Split(" ");
                    HandlePlayerJoined(split1[0], split1[1]);
                    break;
                case "Move":
                    var split2 = value.Split(" ");
                    commandResolver.setCommand(new MoveCommand(gameState, split2[0], split2[1]));
                    break;
                case "Bomb":
                    commandResolver.setCommand(new BombCommand(gameState, this, playerSprites, bombSprites, value));
                    break;
                case "Logs":
                    var logs = JsonConvert.DeserializeObject<List<string>>(value);
                    commandResolver.setCommand(new LogsCommand(gameState, logs));
                    break;
            }

            commandResolver.activate();
        }

        private void HandlePlayerJoined(string id, string userName)
        {
            var pos = gameState.AddEnemy(id, userName);
            if (pos == null)
            {
                return;
            }

            Console.WriteLine("{0}: New player {1} joined with id {2}", gameState.PlayerName, userName, id);
            CreatePlayerSprite(id, userName, pos);
        }

        private void HandlePlayerConnected(string id)
        {
            Console.WriteLine("{0}: connected with id: {1}", gameState.PlayerName, id);

            var pos = gameState.AddOwner(id);
            CreatePlayerSprite(id, gameState.PlayerName, pos);
        }

        private void CreatePlayerSprite(string playerId, string playerName, Vector2f position)
        {
            var bombCount = gameState.getBombsByPlayerId(playerId).Count();
            var point = new Point(position.X * Constants.BLOCK_SIZE, position.Y * Constants.BLOCK_SIZE);
            var sprite = Properties.Resources.character_positioned;
            var playerModel = new PlayerBombsCount(new PlayerName(new PlayerSprite(new PlayerModel(playerName, bombCount, point, sprite))));
            playerSprites[playerId] = (PlayerModel)playerModel.decorate();
            this.Controls.AddRange(playerSprites[playerId].GetControls());
            playerSprites[playerId].BringToFront();
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

            TimeSpan ts = stopwatch.Elapsed;
            gameState.UpdateTick((float)ts.TotalMilliseconds);

            // Update bomb timer labels and remove exploded bombs
            UpdateBombs();
            // Update information on the map based on the new state
            UpdateMap();

            // Remove dead players
            RemoveDeadPlayers(gameState.GetKilledPlayers());

            // Update player bombs count label on sprite
            UpdatePlayerSpriteBombCount(gameState.GetOwnerPlayer());

            // Move player sprites, whose state is not idle
            MoveAnimations();

            // Update inventory
            UpdateInventory();

            // Update fire refrence timers
            UpdateFire();
           
            // Check if final game state has been reached
            var gameStatus = gameState.CheckGameStatus();

            // End the game
            if (this.Visible && (gameStatus == GameStatus.Won || gameStatus == GameStatus.Lost || gameStatus == GameStatus.Tie))
            {
                SendEndGameToServer();
                // Open final form
                string text;
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

                Utils.NewFormOnTop(this, new EndGameForm(text));
            }

            stopwatch.Restart();
        }

        private void RemoveControlsRange(Control[] controls)
        {
            foreach (var control in controls)
            {
                this.Controls.Remove(control);
            }
        }

        private void RemoveDeadPlayers(List<Player> deadPlayers)
        {
            foreach(var player in deadPlayers)
            {
                RemoveControlsRange(playerSprites[player.Id].GetControls());

                playerSprites.Remove(player.Id);

                // Now we can delete this player from the game
                gameState.RemovePlayer(player.Id);
            }
        }

        private void UpdatePlayerSpriteBombCount(Player? player)
        {
            if (player != null)
            {
                playerSprites[player.Id].UpdatePlayerBombsPlaced(gameState.getBombsByPlayerId(player.Id).Count());
            }
        }

        private void UpdateBombs()
        {
            TimeSpan ts = stopwatch.Elapsed;
            // Remove exploded bombs sprites and labels
            var explodedBombs = gameState.UpdateBombTimers((float)ts.TotalMilliseconds);
            foreach (var explodedBomb in explodedBombs)
            {
                RemoveControlsRange(bombSprites[explodedBomb.Id].GetControls());
                bombSprites.Remove(explodedBomb.Id);
            }

            // Update timer labels
            foreach (var bomb in gameState.Bombs)
            {
                bombSprites[bomb.Id].UpdateTimer((int)bomb.Timer);
            }
        }

        private void MoveAnimations()
        {
            var movingPlayers = gameState.GetMovingPlayers();
            foreach (var player in movingPlayers)
            {
                int speed = 5 * player.PlayerSpeed;
                var playerModel = playerSprites[player.Id];

                var dirVector = Utils.GetDirectionVector(player.Direction);
                var nextSpritePos = Utils.AddVectors(
                    new Vector2f(playerModel.Position.X, playerModel.Position.Y),
                    Utils.MultiplyVector(dirVector, speed)
                );
                var nextPlayerPos = Utils.AddVectors(player.Position, dirVector);

                playerModel.UpdatePosition(new Point(nextSpritePos.X, nextSpritePos.Y));

                if (blockmap[nextPlayerPos.Y, nextPlayerPos.X].Location == playerModel.Position)
                {
                    gameState.MovePlayer(player);
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

        private void SendEndGameToServer()
        {
            if (ws != null && ws.ReadyState != WebSocketState.Closed)
            {
                ws.Send("Endgame");
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