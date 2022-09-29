using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bomberman.classes
{
    public class Player
    {
        public PictureBox PlayerSprite { get; set; }
        public int CurrentCollum { get; set; }
        public int CurrentRow { get; set; }
        public Player(PictureBox sprite, int collum,int row)
        {
            PlayerSprite = sprite;
            CurrentCollum = collum;
            CurrentRow = row;
        }
    }
}
