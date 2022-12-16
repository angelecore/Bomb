using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bomberman.classes.Compositetree;
using bomberman.classes.memento;

namespace bomberman.classes.mediator
{
    public class GameManager : IGameManager
    {
        private List<Bomb> bombs = new List<Bomb>();
        private List<Player> players = new List<Player>();
        public List<Component> BombTrees = new List<Component>();

        // key is the player id
        // each player will have a separate player snapshot manager
        public Dictionary<string, PlayerSnapshotManager> PlayerSnapshots = new();

        private float secondTimer = 1.0f;

        private GameState _gameState;
        private ConcreteObserver _gameUI;
        public GameManager(GameState gameState, ConcreteObserver gameUI)
        {
            _gameState = gameState;
            _gameUI = gameUI;
        }

        public void RegisterBomb(Bomb bomb)
        {
            bombs.Add(bomb);
        }

        public void RegisterPlayer(Player player)
        {
            players.Add(player);
        }

        public void ApplyReversePlayerMode(Player initiator)
        {
            var affectedPlayer = players.SingleOrDefault(p => p.Id != initiator.Id);
            if (affectedPlayer == null)
            {
                return;
            }

            PlayerSnapshot? lastSnapshot = null;
            for (int i = 0; i < 5; i++)
            {
                var snapshot = PlayerSnapshots[affectedPlayer.Id].PopLastSnapshot();
                if (snapshot != null)
                {
                    lastSnapshot = snapshot;
                }
            }

            if (lastSnapshot != null)
            {
                affectedPlayer.ApplySnapshot(lastSnapshot);
                _gameUI.SetPlayerPosition(affectedPlayer.Id, affectedPlayer.Position);
            }
        }

        public int GetBombsCountByPlayerId(string playerId)
        {
            return bombs.Count(b => b.Owner.Id == playerId);
        }

        public void PlaceBomb(string playerId)
        {
            var player = players.Find(p => p.Id == playerId);
            if (player == null) return;

            // Can't place a bomb while still moving
            if (player.Direction != Directions.Idle) return;

            // can't place two bombs at the same spot
            foreach (var otherBomb in bombs)
            {
                if (otherBomb.Position.Equals(player.Position))
                {
                    return;
                }
            }

            foreach (var tree in BombTrees)
            {
                foreach (ClusterBomb otherBomb in tree.GetBombs())
                {
                    if (otherBomb.Position.Equals(player.Position))
                    {
                        return;
                    }
                }
            }

            if (player.BombType != BombType.Cluster)
            {
                var bomb = new Bomb(player.Position, player, player.BombExplosionRadius, 0, Guid.NewGuid().ToString(), this);
                bomb.setExplosion(player.BombType);
                RegisterBomb(bomb);
                _gameUI.CreateBombObject(bomb);
            }
            else
            {
                var clusterbomb = new ClusterBomb(player.Position, player, player.BombExplosionRadius, Guid.NewGuid().ToString(), this);
                Composite Tree = new Composite();
                Composite branch = new Composite();
                branch.AddBomb(clusterbomb);
                Tree.AddBomb(branch);
                BombTrees.Add(Tree);
                _gameUI.CreateBombObject(clusterbomb);
            }

        }

        public void ExplodeBomb(Bomb bomb)
        {
            if (bomb.Timer < 1)
            {
                var cells = _gameState.ExplodeBomb(bomb);
                bombs.Remove(bomb);
                _gameUI.RemoveBomb(bomb);

                if (bomb.Owner.BombType == BombType.Cluster && bomb.Generation < 2)
                {
                    foreach (var cell in cells)
                    {
                        bool flag = true;
                        if (cell.Item1.Equals(bomb.Position))
                            continue;
                        Bomb clone = (Bomb)bomb.Clone(cell.Item1, bomb.Generation + 1);
                        foreach (var otherBomb in bombs)
                        {
                            if (otherBomb.Position.Equals(clone.Position))
                            {
                                flag = false;
                            }
                        }
                        if (flag)
                        {
                            bombs.Add(clone);
                            _gameUI.CreateBombObject(clone);
                        }
                    }
                }
            }
        }

        public void UpdateBombTimer(Bomb bomb)
        {
            _gameUI.UpdateBombTimer(bomb);
        }

        public void UpdateBombTimers(float miliSeconds)
        {

            for (int i = bombs.Count - 1; i >= 0; i--)
            {
                bombs[i].UpdateTimer(miliSeconds);
            }

            List<Composite> removeList = new List<Composite>();
            foreach (Composite tree in BombTrees)
            {
                if (tree.Gettimer().Count <= 0)
                {
                    removeList.Add(tree);
                    Console.WriteLine("exloded count" + tree.GetExplodedBombs().Count);
                    continue;
                }

                var timers = tree.updatetimer(miliSeconds);
                int responsecount = tree.GetBranches().Count;
                bool clusterflag = true;
                List<Component> children = new List<Component>();
                foreach (var timer in timers)
                {
                    ClusterBomb bomb = timer.Item2 as ClusterBomb;
                    _gameUI.UpdateBombTimer(bomb);
                    if (timer.Item1 < 1)
                    {
                        if (responsecount > 8 || responsecount > bomb.Radius)
                            clusterflag = false;
                        var cells = _gameState.ExplodeBomb(bomb);
                        
                        _gameUI.RemoveBomb((ClusterBomb)timer.Item2);
                        
                        if (clusterflag)
                        {
                            foreach (var cell in cells)
                            {
                                bool flag = true;
                                if (cell.Item1.Equals(bomb.Position))
                                    continue;
                                ClusterBomb clone = (ClusterBomb)bomb.Clone(cell.Item1);
                                foreach (var otherBomb in bombs)
                                {
                                    if (otherBomb.Position.Equals(clone.Position))
                                    {
                                        flag = false;
                                    }
                                }
                                foreach (ClusterBomb otherBomb in tree.GetBombs())
                                {
                                    if (otherBomb.Position.Equals(clone.Position))
                                    {
                                        flag = false;
                                    }
                                }
                                if (flag)
                                {
                                    children.Add(clone);
                                    _gameUI.CreateBombObject(clone);
                                }
                            }
                        }
                        bomb.notExploded = false;
                    }
                }
                if (children.Count > 0)
                {
                    Composite branch = new Composite();
                    foreach (var child in children)
                    {
                        branch.AddBomb(child);
                    }
                    tree.AddBomb(branch);
                }


            }
            foreach (Composite tree in removeList)
                BombTrees.Remove(tree);
        }

        public void ExplodePosition(Vector2f pos)
        {
            // If another bomb is also is this direction, then also explode this bomb next tic.
            foreach (var anotherBomb in bombs)
            {
                if (anotherBomb.Position.Equals(pos))
                {
                    anotherBomb.Timer = 0;
                }
            }

            foreach (var player in players)
            {
                if (player.Position.Equals(pos))
                {
                    player.IsAlive = false;
                }
            }
        }


        public void UpdateTick(float miliSecondsPassed)
        {
            bool takeSnapshot = false;
            secondTimer -= miliSecondsPassed * 0.001f;
            if (secondTimer <= 0)
            {
                takeSnapshot = true;
                secondTimer = 1.0f;
            }

            UpdatePlayerTimer(miliSecondsPassed, takeSnapshot);
            UpdateBombTimers(miliSecondsPassed);
        }

        private void UpdatePlayerTimer(float miliSecondsPassed, bool takeSnapshot)
        {
            foreach (var player in players)
            {
                player.UpdateTemporaryStats(miliSecondsPassed);

                if (takeSnapshot)
                {
                    player.TakeSnapshot();
                }
            }
        }

        public void AddSnapshot(Player player, PlayerSnapshot snapshot)
        {
            if (!PlayerSnapshots.ContainsKey(player.Id))
            {
                PlayerSnapshots.Add(player.Id, new PlayerSnapshotManager());
            }

            PlayerSnapshots[player.Id].AddSnapshot(snapshot);
        }

    }
}
