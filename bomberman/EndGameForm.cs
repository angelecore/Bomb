using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;

namespace bomberman
{
    public partial class EndGameForm : Form
    {
        private WebSocket ws;

        public EndGameForm(string text, WebSocket ws)
        {
            InitializeComponent(); 
            label1.Text = text;
            label2.Text = "Logs were successfully deleted";
            label2.Visible = false;
            button1.Text = "Delete logs?";
            this.ws = ws;
        }

        private void EndGameForm_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ws != null && ws.ReadyState != WebSocketState.Closed)
            {
                ws.Send("UndoLogs");
            }

            label2.Visible = true;
            button1.Visible = false;
        }
    }
}
