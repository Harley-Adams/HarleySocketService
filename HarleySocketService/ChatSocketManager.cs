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
		private static ConcurrentDictionary<string, WebSocket> SocketClients = new ConcurrentDictionary<string, WebSocket>();

		public string AddSocket(WebSocket socket)
		{
			var id = Guid.NewGuid().ToString();
			SocketClients.TryAdd(id, socket);

			return id;
		}

		public ConcurrentDictionary<string, WebSocket> GetAllSockets()
		{
			return SocketClients;
		}

		public string GetId(WebSocket socket)
		{
			return SocketClients.FirstOrDefault(p => p.Value == socket).Key;
		}

		public async Task RemoveAllSockets()
		{
			foreach(var socket in SocketClients)
			{
				await RemoveSocketAsync(socket.Key);
			}
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
