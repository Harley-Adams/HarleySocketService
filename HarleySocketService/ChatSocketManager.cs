using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HarleySocketService
{
	public class ChatSocketManager
	{
		private ConcurrentDictionary<string, WebSocket> SocketClients = new ConcurrentDictionary<string, WebSocket>();

		public void AddSocket(WebSocket socket)
		{
			SocketClients.TryAdd(Guid.NewGuid().ToString(), socket);
		}

		public async Task RemoveSocketAsync(string id)
		{
			WebSocket socket;
			SocketClients.Remove(id, out socket);

			await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
								statusDescription: "Closed by the ChatSocketManager",
								cancellationToken: CancellationToken.None);
		}

		public async Task SendMessageToAll(string message)
		{
			foreach (var client in SocketClients)
			{
				if (client.Value.State == WebSocketState.Open)
					await SendMessageAsync(client.Value, message);
			}
		}
		private async Task SendMessageAsync(WebSocket socket, string message)
		{
			if (socket.State != WebSocketState.Open)
				return;

			await socket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(message), 0, message.Length),
									WebSocketMessageType.Text,
									true,
									CancellationToken.None);
		}
	}
}
