﻿using System;
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
    public class BasicBombTest
    {

        private readonly ITestOutputHelper output;

        public BasicBombTest(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        public void testBasicBombExplosionPositionsReturn()
        {
            Player player = new Player("temp id", "name", new Vector2f(3, 3));
            player.BombType = BombType.Basic;
            Block[,] grid = new Block[5, 5];
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                    grid[row, col] = new Block(new Vector2f(row, col), BlockType.Empty);
            }
            Bomb bomb = BombFactory.GetBombInstance(player.BombType, player.Position, player, player.BombExplosionRadius);
            List<Vector2f> expected = new List<Vector2f>();
            expected.Add(new Vector2f(3, 3));
            expected.Add(new Vector2f(4, 3));
            expected.Add(new Vector2f(2, 3));
            expected.Add(new Vector2f(3, 4));
            expected.Add(new Vector2f(3, 2));
            GameState gameState = new GameState(player.Name, 1);
            gameState.Width = 5; gameState.Height = 5;
            var cells = bomb.GetExplosionPositions(grid, (pos) => gameState.IsPositionValid(pos));
            List<Vector2f> result = new List<Vector2f>();
            foreach (var cell in cells)
            {
                result.Add(cell.Item1);
            }
            output.WriteLine("This is output from {0}", result.ToArray());
            Assert.Equal(expected, result);
        }
    }
}