using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace bomberman.server
{
    public class Subject
    {
        protected Dictionary<string, Observer> Observers;

        public Subject()
        {
            Observers = new Dictionary<string, Observer>();
        }

        public Dictionary<string, Observer> GetAllObservers()
        {
            return Observers;
        }

        public void Notify(string data)
        {
            foreach(var observer in Observers)
            {
                observer.Value.Update(data);
            }
        }

        public void NotifyOne(string observerId, string data)
        {
            Observers[observerId].Update(data);
        }

        public void Attach(Observer observer)
        {
            Observers.Add(observer.SocketId, observer);
        }

        public void Detach(Observer observer)
        {
            Observers.Remove(observer.SocketId);
        }

    }
}
