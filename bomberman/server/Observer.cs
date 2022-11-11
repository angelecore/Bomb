
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

        private ConcreteSubject _subject;
        
        public Observer(ConcreteSubject subject)
        {
            _subject = subject;
        }

        public void Update(string data)
        {
            Send(data);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            SocketId = Sessions.ActiveIDs.Last();
            _subject.SetState(this, e.Data);
        }
    }
}
