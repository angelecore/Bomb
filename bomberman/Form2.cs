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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (WebSocket ws = new WebSocket("ws://127.0.0.1:7980/Laputa"))
            {
                ws.OnMessage += Ws_OnMessage;
                ws.Connect();
                ws.Send("ha");
                Program.hideForm();
                Form1 form1 = new Form1();
                form1.Show();
                Console.Write('d');
                var xxx = "da";
                /*Console.ReadKey();*/
            }
        }

        private static void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Received - " + e.Data);
        }
    }
}
