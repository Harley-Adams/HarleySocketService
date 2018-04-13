using HarleySocketService.PaperScissorsRock;
using HarleySocketService.PaperScissorsRock.wiremodels;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace HarleySocketService
{
    public class GameInstance
    {
        private ConcurrentDictionary<string, PlayerClient> PlayerClients = new ConcurrentDictionary<string, PlayerClient>();

        private PaperScissorsRockGame GameState;

        public GameInstance(PlayerClient playerOne, PlayerClient playerTwo)
        {
            GameState = new PaperScissorsRockGame(playerOne.GetId(), playerTwo.GetId());
            PlayerClients = new ConcurrentDictionary<string, PlayerClient>();
            PlayerClients.TryAdd(playerOne.GetId(), playerOne);
            PlayerClients.TryAdd(playerTwo.GetId(), playerTwo);
        }

        public void AddPlayer(string id, WebSocket webSocket)
        {
            var player = new PlayerClient(id, webSocket);
            PlayerClients.TryAdd(id, player);
        }

        public void RecievePlayerUpdate(string id, PlayerChoiceEnum Choice)
        {
            PlayerClient client;
            var found = PlayerClients.TryGetValue(id, out client);

            if (found)
            {
                GameState.RecordMove(id, Choice);
            }
        }

        public async Task UpdatePlayersWithGameState()
        {
            var gameStateUpdate = new PaperScissorsRockGameUpdate()
            {
                timeStamp = DateTime.UtcNow.Ticks,
                gameState = GameState.GetWinnerId(),
                scores = GameState.GetScores()
            };

            await BroadcastMessageToPlayersAsync(JsonConvert.SerializeObject(gameStateUpdate));
        }

        public async Task EndGameAsync()
        {
            await BroadcastMessageToPlayersAsync("Game Over");

            foreach (var player in PlayerClients)
            {
                await player.Value.CloseConnectionAsync();
            }
        }

        public ICollection<string> GetPlayerIds()
        {
            return PlayerClients.Keys;
        }

        private async Task BroadcastMessageToPlayersAsync(string message)
        {
            foreach (var player in PlayerClients)
            {
                await player.Value.SendMessageAsync(message);
            }
        }
    }
}
