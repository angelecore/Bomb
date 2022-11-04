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
        WebSocketServer server = new WebSocketServer("ws://127.0.0.1:7980");
        private Dictionary<string, Observer> Observers;
        private List<string> Events;

        public Subject()
        {
            server = new WebSocketServer("ws://127.0.0.1:7980");
            server.AddWebSocketService("/Server", () => new Observer(this));

            Observers = new Dictionary<string, Observer>();
            Events = new List<string>();
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

        public void Attach(Observer observer)
        {
            Observers.Add(observer.SocketId, observer);
        }

        public void AddNewEvent(string userEvent) 
        {
            Events.Add(userEvent);
        }

        public List<string> GetAllEvents()
        {
            return Events;
        }

        public void Run()
        {
            server.Start();

            if (server.IsListening)
            {
                Console.WriteLine("Listening on port {0}, and providing WebSocket services:", server.Port);

                foreach (var path in server.WebSocketServices.Paths)
                    Console.WriteLine("- {0}", path);
            }
        }
    }
}
