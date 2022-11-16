using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using bomberman.classes;

namespace Tests.classes
{
    public class GameStateTest
    {

        [Fact]
        public void test_CheckGameStatus_Returns_GameStatusObject()
        {
            GameState gamestate = new GameState("test");
            var gameStatus = gamestate.CheckGameStatus();
            Assert.NotNull(gameStatus);
        }

        [Fact]
        public void test_CheckGameStatus_Returns_WaitingForPlayers_On1Player()
        {
            GameState gamestate = new GameState("test");
            GameStatus gameStatus = gamestate.CheckGameStatus();
            GameStatus ExpectedStatus = GameStatus.WaitingForPlayers;
            Assert.Equal(ExpectedStatus, gameStatus);
        }

        [Fact]
        public void test_CheckGameStatus_Returns_InProgress_On2Player()
        {
            GameState gamestate = new GameState("test");
            gamestate.AddOwner("test");
            gamestate.AddEnemy("testid", "test2");
            GameStatus gameStatus = gamestate.CheckGameStatus();
            GameStatus ExpectedStatus = GameStatus.InProgress;
            Assert.Equal(ExpectedStatus, gameStatus);
        }

        [Fact]
        public void test_CheckGameStatus_Returns_Tie()
        {
            GameState gamestate = new GameState("test");
            gamestate.AddOwner("test");
            gamestate.AddEnemy("testid", "test2");
            gamestate.CheckGameStatus();
            gamestate.RemovePlayer("test");
            gamestate.RemovePlayer("testid");
            GameStatus gameStatus = gamestate.CheckGameStatus();
            GameStatus ExpectedStatus = GameStatus.Tie;
            Assert.Equal(ExpectedStatus, gameStatus);
        }

        [Fact]
        public void test_CheckGameStatus_Returns_Won()
        {
            GameState gamestate = new GameState("test");
            gamestate.AddOwner("test");
            gamestate.AddEnemy("testid", "test2");
            gamestate.CheckGameStatus();
            gamestate.RemovePlayer("testid");
            GameStatus gameStatus = gamestate.CheckGameStatus();
            GameStatus ExpectedStatus = GameStatus.Won;
            Assert.Equal(ExpectedStatus, gameStatus);
        }

        [Fact]
        public void test_RemoveExplodedTiles_BasicBomb_DestroyBlock()
        {
            GameState gameState = new GameState("TestName");
            Player TestPlayer = new Player("TestId", "TestName", new Vector2f(1, 1));
            FireController TestFireController = new FireController("id", new Fire("id"), 3, 3);
            Block[,] grid = new Block[3, 3];
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                    grid[row, col] = new Block(new Vector2f(row, col), BlockType.Destructable);
            }
            gameState.Grid = grid;
            Vector2f TestCoordinates = new Vector2f(1,1);
            List<Tuple<Vector2f, int>> list = new List<Tuple<Vector2f, int>>();
            list.Add(new Tuple<Vector2f,int>(TestCoordinates, 1));
            gameState.RemoveExplodedTiles(list, TestPlayer, TestFireController);
            BlockType Expectedresult  = BlockType.Empty;
            BlockType RealBlockType = gameState.Grid[1, 1].Type;
            Assert.Equal(Expectedresult, RealBlockType);
        }

        [Fact]
        public void test_RemoveExplodedTiles_FireBomb_Fire()
        {
            GameState gameState = new GameState("TestName");
            Player TestPlayer = new Player("TestId", "TestName", new Vector2f(1, 1));
            TestPlayer.BombType = BombType.Fire;
            FireController TestFireController = new FireController("id", new Fire("id"), 3, 3);
            Block[,] grid = new Block[3, 3];
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                    grid[row, col] = new Block(new Vector2f(row, col), BlockType.Destructable);
            }
            gameState.Grid = grid;
            Vector2f TestCoordinates = new Vector2f(1, 1);
            List<Tuple<Vector2f, int>> list = new List<Tuple<Vector2f, int>>();
            list.Add(new Tuple<Vector2f, int>(TestCoordinates, 1));
            gameState.RemoveExplodedTiles(list, TestPlayer, TestFireController);
            BlockType Expectedresult = BlockType.Fire;
            BlockType RealBlockType = gameState.Grid[1, 1].Type;
            Assert.Equal(Expectedresult, RealBlockType);
        }

        [Fact]
        public void test_UpdateBombTimers_1second()
        {
            GameState gameState = new GameState("TestName");
            Player TestPlayer = new Player("TestId", "TestName", new Vector2f(1, 1));
            gameState.Bombs.Add(new BasicBomb(new Vector2f(1, 1),TestPlayer,2,0));
            gameState.UpdateBombTimers(1000);
            float ExpectedTimer = 7.0f;
            float RealTimer = 0f;
            foreach(Bomb bomb in gameState.Bombs)
                RealTimer = bomb.Timer;
            Assert.Equal(ExpectedTimer, RealTimer);

        }

        [Fact]
        public void test_AddOwner_replace()
        {
            GameState gameState = new GameState("Replacable");
            Player TestPlayer = new Player("TestId", "TestName", new Vector2f(1, 1));
            gameState.AddOwner("TestId");
            Assert.Equal("TestId",gameState.PlayerId);

        }

        [Fact]
        public void test_GetOwnerPlayer_Equals()
        {
            Player Owner = new Player("TestId","TestName",new Vector2f(1,1));
            GameState gameState = new GameState(Owner.Name);
            gameState.AddOwner(Owner.Id);
            var real = gameState.GetOwnerPlayer();
            Assert.Equal(Owner.Id,real.Id);
        }

        [Fact]
        public void test_PlaceBomb_InvalidPlayerId()
        {
            Player Test = new Player("TestId","TestName",new Vector2f(1,1));
            GameState gameState = new GameState(Test.Name);
            var real = gameState.PlaceBomb("null");
            Assert.Null(real);
        }

        [Fact]
        public void test_PlaceBomb_TwoBombInSameSpot()
        {
            Player Test = new Player("TestId", "TestName", new Vector2f(1, 1));
            GameState gameState = new GameState(Test.Name);
            gameState.AddOwner(Test.Id);
            gameState.PlaceBomb(Test.Id);
            var real = gameState.PlaceBomb(Test.Id);
            Assert.Null(real);
        }

        [Fact]
        public void test_PlaceBomb_Valid()
        {
            Player Test = new Player("TestId", "TestName", new Vector2f(1, 1));
            GameState gameState = new GameState(Test.Name);
            gameState.AddOwner(Test.Id);
            var real = gameState.PlaceBomb(Test.Id);
            Assert.NotNull(real);
        }
    }
}
