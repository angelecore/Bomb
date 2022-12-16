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

        string id;

        public BombCommand(
            GameState gameState,
            string id
        ) {
            this.gameState = gameState;
            this.id = id;
        }

        public void execute()
        {
            gameState.PlaceBomb(id);
        }

        public void undo()
        {
            return;
        }
    }
}
