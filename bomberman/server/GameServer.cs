using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace bomberman.server
{
    public class GameServer : WebSocketBehavior
    {
        GameNetwork _gameNetwork;

        public GameServer(GameNetwork gameNetwork)
        {
            _gameNetwork = gameNetwork;
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            // send logs
            if (e.Data.Contains("Endgame"))
            {
                Send(String.Format("Logs {0}", JsonConvert.SerializeObject(_gameNetwork.GetAllEvents())));
                return;
            }

            _gameNetwork.AddNewEvent(e.Data);

            if (e.Data.Contains("Connected"))
            {
                string userName = e.Data.Split(' ')[1];

                var users = _gameNetwork.GetAllUsers();
                foreach (var p in Sessions.ActiveIDs.ToArray().SubArray(0, Sessions.ActiveIDs.Count() - 1))
                {
                    Send(string.Format("Joined {0} {1}", p.ToString(), users[p.ToString()].UserName));
                }
                
                _gameNetwork.AddNewUser(Sessions.ActiveIDs.Last(), userName);
                // the user itself only needs to know of it's session id.
                Send(string.Format("Connected {0}", Sessions.ActiveIDs.Last()));

                Sessions.Broadcast(string.Format("Joined {0} {1}", Sessions.ActiveIDs.Last(), userName));
            }
            else
            {
                Sessions.Broadcast(e.Data);
            }
        }
    }
}
