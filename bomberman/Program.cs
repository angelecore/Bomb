using WebSocketSharp;
using WebSocketSharp.Server;

namespace bomberman
{
    public class Laputa : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine(e.Data);
        }
    }

    internal class Program
    {
        public static Form2? Form2;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Form2 = new Form2();

            try
            {
                WebSocketServer wssv = new WebSocketServer("ws://127.0.0.1:7980");
                wssv.AddWebSocketService<Laputa>("/Laputa");
                wssv.Start();

                if (wssv.IsListening)
                {
                    Console.WriteLine("Listening on port {0}, and providing WebSocket services:", wssv.Port);

                    foreach (var path in wssv.WebSocketServices.Paths)
                        Console.WriteLine("- {0}", path);
                }
            }
            catch (Exception ex)
            {
            }

            Application.Run(Form2);
        }

        public static void hideForm()
        {
            if (Form2 != null)
            {
                Form2.Hide();
            }
        }

        private static void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Received - " + e.Data);
        }
    }
}