using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.client
{
    internal interface IModel
    {
        public Point Position { get; set; }
        public void BringToFront();
        public Control[] GetControls();
        public void UpdatePosition(Point p);
    }
}
