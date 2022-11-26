using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.interpreter
{
    public class Interpreter
    {
        private bool isInInterpreterMode;
        private ConcreteObserver form;
        public Interpreter(ConcreteObserver form)
        {
            this.form = form;
            isInInterpreterMode = false;
        }

        public void Execute(string command)
        {
            if (command == null)
            {
                return;
            }

            Interpret(command.Split(' '));
        }

        public bool active()
        {
            return isInInterpreterMode;
        }

        public void changeInterpreterMode()
        {
            isInInterpreterMode = !isInInterpreterMode;
        }

        private void Interpret(string[] splitCommands)
        {
            switch(splitCommands[0])
            {
                case "QUIT":
                    Environment.Exit(1);
                    break;
                case "WIN":
                    getGameStateOfPlayer(splitCommands[1]).setGameStatus(GameStatus.Won);
                    getGameStateOfPlayer(splitCommands[1], true).setGameStatus(GameStatus.Lost);

                    break;
                case "SPEED":
                    var consolePlayer = getGameStateOfPlayer(splitCommands[1]).GetOwnerPlayer();
                    consolePlayer.AddNewStat(new PlayerStatsBuilder()
                        .WithTimer(9999)
                        .WithAddSpeed(1)
                        .Build()
                    );

                    break;
            }

            this.isInInterpreterMode = false;
        }

        private GameState getGameStateOfPlayer(string id, bool secondPlayer = false)
        {
            return Application.OpenForms
                .OfType<ConcreteObserver>()
                .ToList()
                .First(
                    secondPlayer ? 
                    x => !x.Equals(form) :
                    x => x.getGameState().PlayerId == id
                ).getGameState();
        }
    }
}
