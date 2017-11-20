using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HarleySocketService
{
	public class Startup
    {
		private static List<WebSocket> ClientConnections = new List<WebSocket>();

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
			services.AddSingleton<ChatSocketManager>();
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

			app.UseWebSockets();
			app.UseMiddleware<ChatSocketMiddleware>();

			app.Run(async (context) =>
            {
					await context.Response.WriteAsync("Hello World! Use the ?reset query param to drop all current websocket connections if run out of azure connections");
					ClientConnections.Clear();
			});
        }
	}
}
