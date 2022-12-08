using bomberman.classes;
using WebSocketSharp;

namespace bomberman
{
    public partial class JoinForm : Form
    {
        public JoinForm(int leftOffset, int topOffset)
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Left = leftOffset;
            this.Top = topOffset;
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
            Utils.NewFormOnTop(this, new ConcreteObserver(textBox1.Text));
        }

        private static void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Received - " + e.Data);
        }

        private void frm_menu_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
    }
}
