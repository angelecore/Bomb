using bomberman.client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public class BombCommand : ICommand
    {
        GameState gameState;

        Form1 form;

        Dictionary<string, PlayerModel> playerSprites;

        Dictionary<string, BombModel> bombSprites;

        string id;

        public BombCommand(
            GameState gameState,
            Form1 form,
            Dictionary<string, PlayerModel> playerSprites,
            Dictionary<string, BombModel> bombSprites,
            string id
        ) {
            this.gameState = gameState;
            this.form = form;
            this.playerSprites = playerSprites;
            this.bombSprites = bombSprites;
            this.id = id;
        }

        public void execute()
        {
            if (gameState.CheckGameStatus() == GameStatus.WaitingForPlayers)
            {
                return;
            }

            var bomb = gameState.PlaceBomb(id);
            if (bomb == null) return;
            bombSprites[bomb.Id] = new BombModel((int)bomb.Timer, new Point(bomb.Position.X * Constants.BLOCK_SIZE, bomb.Position.Y * Constants.BLOCK_SIZE), Properties.Resources.bombSprite);
            form.setBombSprites(bombSprites);
            form.Controls.AddRange(bombSprites[bomb.Id].GetControls());
            bombSprites[bomb.Id].BringToFront();
            playerSprites[id].BringToFront();
            
        }
    }
}
