using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.server
{
    public class User
    {
        public string SocketId { get; set; }
        public string UserName { get; set; }

        public User(string socketId, string userName)
        {
            SocketId = socketId;
            UserName = userName;
        }   
    }
}
