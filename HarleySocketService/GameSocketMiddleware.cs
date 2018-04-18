using HarleySocketService.PaperScissorsRock;
using HarleySocketService.PaperScissorsRock.Wiremodels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
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
                ActiveGames.Clear();

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

            if (userId == null)
            {
                userId = Guid.NewGuid().ToString();
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            var playerClient = new PlayerClient(userId, socket);

            if (WaitingPlayers.Count == 0)
            {
                await AddPlayerToWaitList(playerClient);
            }
            else
            {
                var otherPlayer = WaitingPlayers.FirstOrDefault();
                await StartMatch(otherPlayer, playerClient);
            }

            await Receive(socket, async (result, buffer) =>
            {
                var game = ActiveGames.FirstOrDefault(g => g.GetPlayerIds().Contains(userId));

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    if (game != null)
                    {
                        await game.EndGameAsync();
                        ActiveGames.Remove(game);
                    }

                    return;
                }

                if (game == null)
                {
                    if (!WaitingPlayers.Contains(playerClient))
                    {
                        WaitingPlayers.Add(playerClient);
                    }
                    return;
                }

                var bytesAsString = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var playerMessage = JsonConvert.DeserializeObject<PlayerMessage>(bytesAsString);

                if(playerMessage.messageType == PlayerMessageTypeEnum.PaperScissorsRockChoice)
                {
                    var playerChoice = JsonConvert.DeserializeObject<PaperScissorsRockPlayerMessage>(bytesAsString);
                    game.RecievePlayerUpdate(userId, playerChoice.choice);
                }

                await game.UpdatePlayersWithGameState();
            });
        }

        private async Task AddPlayerToWaitList(PlayerClient player)
        {
            WaitingPlayers.Add(player);

            var gameStateUpdate = new PaperScissorsRockGameUpdate()
            {
                timeStamp = DateTime.UtcNow.Ticks,
                gameState = GameStateEnum.Lobby,
                scores = { },
                winnerId = null
            };

            await player.SendMessageAsync(JsonConvert.SerializeObject(gameStateUpdate));
        }

        private async Task StartMatch(PlayerClient playerOne, PlayerClient playerTwo)
        {
            var newGame = new GameInstance(playerOne, playerTwo);
            WaitingPlayers.Remove(playerOne);

            ActiveGames.Add(newGame);

            await newGame.UpdatePlayersWithGameState();

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
