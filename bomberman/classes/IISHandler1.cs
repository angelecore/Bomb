/*using Microsoft.AspNetCore.Http;
using ServiceStack.Host;
using System;
using System.Web;

namespace bomberman.classes
{
    public class IISHandler1 : IHttpHandler
    {
        /// <summary>
        /// You will need to configure this handler in the Web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: https://go.microsoft.com/?linkid=8101007
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            if (context.IsWebSocketRequest)
            {
                context.AcceptWebSocketRequest(EchoWebSocket);
            }
        }

        private async Task EchoWebSocket(AspNetWebSocketContext socketContext)
        {
            var buffer = new byte[1024 * 4];
            var result = await socketContext.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                await socketContext.WebSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                result = await socketContext.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await socketContext.WebSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
*/