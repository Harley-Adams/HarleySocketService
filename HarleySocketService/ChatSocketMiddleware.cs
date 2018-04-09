using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HarleySocketService
{
    public class ChatSocketMiddleware
    {
		private readonly RequestDelegate Next;
		private readonly ChatSocketManager SocketManager;
        
		public ChatSocketMiddleware(ChatSocketManager socketManager, RequestDelegate next)
		{
			SocketManager = socketManager;
			Next = next;
		}

		public async Task Invoke(HttpContext context)
		{
            if (!context.Request.Query.ContainsKey("chat"))
            {
                await Next.Invoke(context);
                return;
            }
            if (context.Request.Query.ContainsKey("reset"))
			{
				await SocketManager.RemoveAllSockets();
				await Next.Invoke(context);
				return;
			}
			if (!context.WebSockets.IsWebSocketRequest)
			{
				await Next.Invoke(context);
				return;
			}

			var socket = await context.WebSockets.AcceptWebSocketAsync();
			var id = SocketManager.AddSocket(socket);

			await Receive(socket, async (result, buffer) =>
			{
				if (result.MessageType == WebSocketMessageType.Close)
				{
					await SocketManager.RemoveSocketAsync(id);
					return;
				}

				var responseString = Encoding.ASCII.GetString(buffer, 0, result.Count);
				await SocketManager.SendMessageToAll(responseString);
			});
		}

		private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
		{
			var buffer = new byte[1024 * 4];

			while (socket.State == WebSocketState.Open)
			{
				var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
														cancellationToken: CancellationToken.None);

				handleMessage(result, buffer);
			}
		}
	}
}
