using bomberman.classes;
using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bomberman.classes;
using System.Drawing.Text;
using bomberman.classes.facade;
using System.Numerics;

namespace Tests.classes
{
    public class playerMovementFacadeTest
    {
        [Fact]
        public void CanSetDirectionTest()
        {
            Player player = new Player("temp id", "name", new Vector2f(4, 4));
            Block[,] Grid = new Block[5, 5];
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                    Grid[row, col] = new Block(new Vector2f(row, col), BlockType.Empty);
            }
            string action = "Up";
            int Width = 5;
            int Height = 5;
            PlayerMovementFacade playerMovementFacade = new PlayerMovementFacade(player, action, Width, Height, Grid);
            Assert.True(playerMovementFacade.canSetDirection());
        }
    }
}
