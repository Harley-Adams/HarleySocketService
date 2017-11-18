using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;
using System.Threading;

namespace HarleySocketService
{
    public class Startup
    {
		private List<WebSocket> ClientConnections = new List<WebSocket>();

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

			app.UseWebSockets();

			app.Run(async (context) =>
            {
				if(context.WebSockets.IsWebSocketRequest)
				{
					WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
					//await Echo(context, webSocket);
					await Chat(context, webSocket);
				}
				else
				{
					await context.Response.WriteAsync("Hello World!");
				}
			});
        }

		private async Task Echo(HttpContext context, WebSocket webSocket)
		{
			var buffer = new byte[1024 * 4];
			WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			while (!result.CloseStatus.HasValue)
			{
				await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

				result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			}
			await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
		}

		private async Task Chat(HttpContext context, WebSocket webSocket)
		{
			var buffer = new byte[1024 * 4];
			WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

			ClientConnections.Add(webSocket);
			while (!result.CloseStatus.HasValue)
			{
				foreach(var clientSocket in ClientConnections)
				{
					await clientSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
				}

				result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

			}
			await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
		}
	}
}
