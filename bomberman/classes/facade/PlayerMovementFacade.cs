using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.facade
{
    public class PlayerMovementFacade
    {
        Directions direction;

        Player player;

        string action;

        CheckPlayerMoving checkPlayerMoving;

        CheckIsEmptyPath checkIsEmptyPath;

        CheckPositionValid checkPositionValid;

        GameState gameState;

        public PlayerMovementFacade(Player player, string action, Block[,] grid, GameState gameState)
        {
            this.direction = Directions.Idle;
            this.player = player;
            this.action = action;
            this.checkPlayerMoving = new CheckPlayerMoving();
            this.checkIsEmptyPath = new CheckIsEmptyPath(grid);
            this.checkPositionValid = new CheckPositionValid(
                GameDataSingleton.GetInstance().Width,
                GameDataSingleton.GetInstance().Height
            );
            this.gameState = gameState;
        }

        public bool canSetDirection()
        {
            switch (action)
            {
                case "Up":
                    direction = Directions.Up;
                    break;
                case "Down":
                    direction = Directions.Down;
                    break;
                case "Left":
                    direction = Directions.Left;
                    break;
                case "Right":
                    direction = Directions.Right;
                    break;
            }

            var dirVec = Utils.GetDirectionVector(direction);

            return checkPlayerMoving.IsValid(player, dirVec) && checkIsEmptyPath.IsValid(player, dirVec) && 
                checkPositionValid.IsValid(player, dirVec);
        }

        public Directions getNewDirection()
        {
            return direction;
        }

        public void movePlayer()
        {
            if (player.Direction != Directions.Idle && player.Direction != direction)
            {
                gameState.MovePlayer(player);
            }

            player.SetDirection(direction);
        }
    }
}
