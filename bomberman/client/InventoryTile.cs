using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.client
{
    internal class InventoryTile
    {
        public string Text { get; set; }

        public string? OwnershipId { get; set; }
        private Label label { get; set; }
        private Point Position { get; set; }
        public InventoryTile(string text, Point position, string ?ownershipId = null)
        {
            Text = text; 
            Position = position;
            OwnershipId = ownershipId;


            label = new Label();
            label.Location = Position;
            label.Size = new Size(200, 20);
            label.Text = text;
            label.BackColor = Color.Transparent;
        }

        public void UpdateText(string newText)
        {
            Text = newText;
            label.Text = newText;
        }

        public Control[] GetControls()
        {
            return new Control[] {label};
        }
    }
}
