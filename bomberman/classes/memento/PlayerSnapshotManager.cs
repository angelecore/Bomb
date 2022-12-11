using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.memento
{
    public class PlayerSnapshotManager
    {
        private Stack<PlayerSnapshot> _snapshots;
        public PlayerSnapshotManager()
        {
            _snapshots = new Stack<PlayerSnapshot>();
        }

        public void AddSnapshot(PlayerSnapshot snapshot)
        {
            _snapshots.Push(snapshot);
        }

        public PlayerSnapshot? PopLastSnapshot()
        {
            if (_snapshots.Count == 0)
            {
                return null;
            }
            return _snapshots.Pop();
        }
    }
}
