using HarleySocketService.PaperScissorsRock;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HarleySocketService
{
    public class GameSocketMiddleware
    {
        private readonly RequestDelegate Next;
        private readonly List<GameInstance> ActiveGames;
        private readonly List<PlayerClient> WaitingPlayers;

        public GameSocketMiddleware(RequestDelegate next)
        {
            Next = next;
            ActiveGames = new List<GameInstance>();
            WaitingPlayers = new List<PlayerClient>();
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Query.ContainsKey("game"))
            {
                await Next.Invoke(context);
                return;
            }

            if (context.Request.Query.ContainsKey("reset"))
            {
                foreach (var game in ActiveGames)
                {
                    await game.EndGameAsync();
                }
                foreach (var player in WaitingPlayers)
                {
                    await player.CloseConnectionAsync();
                }

                return;
            }

            if (!context.WebSockets.IsWebSocketRequest)
            {
                await Next.Invoke(context);
                return;
            }

            string userId = null;

            if (context.Request.Query.ContainsKey("userid"))
            {
                StringValues stringValues;
                var success = context.Request.Query.TryGetValue("userid", out stringValues);
                userId = success ? stringValues.First() : Guid.NewGuid().ToString();
            }

            if(userId == null)
            {
                userId = Guid.NewGuid().ToString();
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            await Receive(socket, async (result, buffer) =>
            {
                var game = ActiveGames.FirstOrDefault(g => g.GetPlayerIds().Contains(userId));


                if (result.MessageType == WebSocketMessageType.Close)
                {
                    if (game != null)
                    {
                        await game.EndGameAsync();
                    }
                    return;
                }

                if(game == null)
                {
                    if(WaitingPlayers.Count > 0)
                    {
                        var playerOne = WaitingPlayers.FirstOrDefault();
                        var newGame = new GameInstance(playerOne, new PlayerClient(userId, socket));
                        WaitingPlayers.Remove(playerOne);

                        ActiveGames.Add(newGame);
                        await newGame.BroadcastGameStateAsync("test");

                        return;
                    }
                    else
                    {
                        WaitingPlayers.Add(new PlayerClient(userId, socket));

                        return;
                    }
                }

                game.RecievePlayerUpdate(userId, PlayerChoiceEnum.Paper);

                await game.BroadcastGameStateAsync("game state placeholder");
                //var responseString = Encoding.ASCII.GetString(buffer, 0, result.Count);
                //await SocketManager.SendMessageToAll(responseString);
            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}
