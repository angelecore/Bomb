using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public enum GameStatus
    {
        WaitingForPlayers,
        InProgress,
        Lost,
        Tie,
        Won
    }

    internal class GameDataSingleton
    {
        public GameStatus CurrentGameStatus;
        public int Width { get; set; }
        public int Height { get; set; }
        public int MaxPlayerCount { get; set; }

        private GameDataSingleton() { }

        private static GameDataSingleton instance;

        public static GameDataSingleton GetInstance()
        {
            if (instance == null)
            {
                instance = new GameDataSingleton();
            }

            return instance;
        }

        public void SetCurrentGameStatus(GameStatus gameStatus)
        {
            CurrentGameStatus = gameStatus;
        }

        public void SetWidth(int width)
        {
            Width = width;
        }

        public void SetHeight(int height)
        {
            Height = height;
        }

        public void SetMaxPlayerCount(int count)
        {
            MaxPlayerCount = count;
        }
    }
}
