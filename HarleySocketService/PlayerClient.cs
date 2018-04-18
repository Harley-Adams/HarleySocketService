using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HarleySocketService
{
    public class PlayerClient
    {
        private WebSocket ClientSocket { get; set; }
        private string Id;

        public PlayerClient(string id, WebSocket socket)
        {
            Id = id;
            ClientSocket = socket;
        }

        public string GetId()
        {
            return Id;
        }

        public async Task SendMessageAsync(string message)
        {
            if (ClientSocket.State != WebSocketState.Open)
                return;

            await ClientSocket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(message), 0, message.Length),
                                    WebSocketMessageType.Text,
                                    true,
                                    CancellationToken.None);
        }

        public async Task CloseConnectionAsync()
        {
            if(ClientSocket.State != WebSocketState.Closed)
            {
                await ClientSocket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                        statusDescription: "Closed by the GameServer",
                        cancellationToken: CancellationToken.None);
            }
        }
    }
}
