using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;
using Newtonsoft.Json;

namespace bomberman.server
{
    public class ConcreteSubject : Subject
    {
        WebSocketServer server = new WebSocketServer("ws://127.0.0.1:7980");
        public ConcreteSubject() : base()
        {
            server = new WebSocketServer("ws://127.0.0.1:7980");
            server.AddWebSocketService("/Server", () => new Observer(this));
        }

        public void SetState(Observer observer, string message)
        {
            var observerId = observer.SocketId;
            // send logs
            if (message.Contains("Endgame"))
            {
                NotifyOne(observerId, String.Format("Logs {0}", JsonConvert.SerializeObject(GetState())));
                return;
            }

            LogsSingleton.GetInstance().Add(message);

            if (message.Contains("Connected"))
            {
                observer.UserName = message.Split(' ')[1];
                Attach(observer);

                foreach (var p in Observers.Where(o => o.Key != observer.SocketId))
                {
                    NotifyOne(observerId, string.Format("Joined {0} {1}", p.Key, p.Value.UserName));
                }


                // the user itself only needs to know of it's session id.
                NotifyOne(observerId, string.Format("Connected {0}", observerId));

                // Notify other user that I have joined
                Notify(string.Format("Joined {0} {1}", observerId, observer.UserName));
            }
            else
            {
                Notify(message);
            }
        }

        public List<string>? GetState()
        {
            return LogsSingleton.GetInstance().All();
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
