using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace bomberman.server
{
    public class GameNetwork
    {
        WebSocketServer server = new WebSocketServer("ws://127.0.0.1:7980");
        private Dictionary<string, User> Users;

        public GameNetwork()
        {
            server = new WebSocketServer("ws://127.0.0.1:7980");
            server.AddWebSocketService("/Server", () => new GameServer(this));

            Users = new Dictionary<string, User>();
        }

        public Dictionary<string, User> GetAllUsers()
        {
            return Users;
        }

        public void AddNewUser(string socketId, string name)
        {
            Users.Add(socketId, new User(socketId, name));
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
