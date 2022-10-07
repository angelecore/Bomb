using WebSocketSharp;
using WebSocketSharp.Server;

namespace bomberman
{
    public class Laputa : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data == "BALUS"
                      ? "Are you kidding?"
                      : "I'm not available now.";

            Send(e.Data);
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

            WebSocketServer wssv = new WebSocketServer("ws://127.0.0.1:7980");
            wssv.AddWebSocketService<Laputa>("/Laputa");
            wssv.Start();

            if (wssv.IsListening)
            {
                Console.WriteLine("Listening on port {0}, and providing WebSocket services:", wssv.Port);

                foreach (var path in wssv.WebSocketServices.Paths)
                    Console.WriteLine("- {0}", path);
            }

            Application.Run(Form2);

            Console.WriteLine("\nPress Enter key to stop the server...");
            Console.ReadKey(true);

            wssv.Stop();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //ApplicationConfiguration.Initialize();
            //Application.Run(new Form1());
            /*using (WebSocket ws = new WebSocket("ws://127.0.0.1:7980"))
            {
                ws.OnMessage += Ws_OnMessage;
                ws.Connect();
                ws.Send("Hello");
                Console.ReadKey();
            }*/
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