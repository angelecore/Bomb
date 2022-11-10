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
    public class Observer : WebSocketBehavior
    {
        public string SocketId { get; set; }
        public string UserName { get; set; }

        private Subject _gameNetwork;
        
        public Observer(Subject gameNetwork)
        {
            _gameNetwork = gameNetwork;
        }

        public void Update(string data)
        {
            Send(data);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            // send logs
            if (e.Data.Contains("Endgame"))
            {
                Send(String.Format("Logs {0}", JsonConvert.SerializeObject(_gameNetwork.GetAllEvents())));
                return;
            }

            if (e.Data.Contains("UndoLogs"))
            {
                Send(String.Format("UndoLogs {0}", e.Data));
                return;
            }

            _gameNetwork.AddNewEvent(e.Data);

            if (e.Data.Contains("Connected"))
            {
                string userName = e.Data.Split(' ')[1];

                var users = _gameNetwork.GetAllObservers();
                foreach (var p in Sessions.ActiveIDs.ToArray().SubArray(0, Sessions.ActiveIDs.Count() - 1))
                {
                    Send(string.Format("Joined {0} {1}", p.ToString(), users[p.ToString()].UserName));
                }


                this.SocketId = Sessions.ActiveIDs.Last();
                this.UserName = userName;

                _gameNetwork.Attach(this);
                
                // the user itself only needs to know of it's session id.
                Send(string.Format("Connected {0}", Sessions.ActiveIDs.Last()));

                _gameNetwork.Notify(string.Format("Joined {0} {1}", Sessions.ActiveIDs.Last(), userName));
               // Sessions.Broadcast();
            }
            else
            {
                Sessions.Broadcast(e.Data);
            }
        }
    }
}
