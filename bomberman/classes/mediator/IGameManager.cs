using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bomberman.classes.memento;

namespace bomberman.classes.mediator
{
    public interface IGameManager
    {
        public void RegisterBomb(Bomb bomb);
        public void RegisterPlayer(Player player);
        public void ApplyReversePlayerMode(Player initiator);
        public int GetBombsCountByPlayerId(string playerId);
        public void PlaceBomb(string playerId);
        public void ExplodeBomb(Bomb bomb);
        public void UpdateBombTimer(Bomb bomb);
        public void ExplodePosition(Vector2f pos);
        public void UpdateTick(float miliSecondsPassed);
        public void AddSnapshot(Player player, PlayerSnapshot snapshot);
        public void KillPlayer(Vector2f pos);
        public int GetPlayerCount();
        public Player? GetPlayerById(string playerId);
        public List<Player> GetMovingPlayers();

    }
}
