using bomberman.client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bomberman.classes.mediator;

namespace bomberman.classes
{
    public class BombCommand : ICommand
    {
        IGameManager gameManager;

        string id;

        public BombCommand(
            IGameManager gameManager,
            string id
        ) {
            this.gameManager = gameManager;
            this.id = id;
        }

        public void execute()
        {
            gameManager.PlaceBomb(id);
        }

        public void undo()
        {
            return;
        }
    }
}
