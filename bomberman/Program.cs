using WebSocketSharp;
using WebSocketSharp.Server;

namespace bomberman
{
    public class Laputa : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("Server: {0}", e.Data);

            if (e.Data.Contains("Connected"))
            {
                foreach (var p in Sessions.ActiveIDs.ToArray().SubArray(0, Sessions.ActiveIDs.Count() - 1))
                {
                    Send(string.Format("Joined {1}", e.Data, p.ToString()));
                }
                Send(string.Format("Connected {1}", e.Data, Sessions.ActiveIDs.Last()));
                Sessions.Broadcast(string.Format("Joined {1}", e.Data, Sessions.ActiveIDs.Last()));
            } else
            {
                Sessions.Broadcast(e.Data);
            }
        }
    }

    public class MultiFormContext : ApplicationContext
    {
        private int openForms;
        public MultiFormContext(params Form[] forms)
        {
            openForms = forms.Length;

            foreach (var form in forms)
            {
                /*
                form.FormClosed += (s, args) =>
                {
                    //When we have closed the last of the "starting" forms, 
                    //end the program.
                    if (Interlocked.Decrement(ref openForms) == 0)
                        ExitThread();
                };
                */

                form.Show();
            }
        }
    }

    internal class Program
    {
        public static Form2? P1, P2;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

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

            P1 = new Form2();
            P2 = new Form2();

            var context = new MultiFormContext(new Form2(), new Form2());
            Application.Run(context);
        }

        private static void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Received - " + e.Data);
        }
    }
}