using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using bomberman.classes;
using Moq;
using Xunit.Abstractions;


namespace Tests.classes.BombTests
{
    public class FireBombTest
    {

        private readonly ITestOutputHelper output;

        public FireBombTest(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        public void testFireBombExplosionPositionsReturn()
        {
            Player player = new Player("temp id", "name", new Vector2f(3, 3));
            player.BombType = BombType.Fire;
            Block[,] grid = new Block[5, 5];
            for(int row = 0; row < 5; row++)
            {
                for(int col = 0; col < 5; col++)
                    grid[row, col] = new Block(new Vector2f(row,col),BlockType.Empty);
            }
            Bomb bomb = new Bomb(player.Position, player, 2, 0);
            bomb.setExplosion(BombType.Fire);
            List<Vector2f> expected = new List<Vector2f>();
            expected.Add(new Vector2f(3,3));
            expected.Add(new Vector2f(4, 3));
            expected.Add(new Vector2f(2, 3));
            expected.Add(new Vector2f(3, 4));
            expected.Add(new Vector2f(3, 2));
            GameState gameState = new GameState(player.Name, 1);
            var cells = bomb.GetExplosionPositions(grid, (pos) => gameState.IsPositionValid(pos));
            List<Vector2f> result = new List<Vector2f>();
            foreach(var cell in cells)
            {
                result.Add(cell.Item1);
            }
            output.WriteLine("This is output from {0}", result.ToArray());
            Assert.Equal(expected, result);
        }
    }
}
