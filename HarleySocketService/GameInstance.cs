using HarleySocketService.PaperScissorsRock;
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
            GameState = new PaperScissorsRockGame();
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

            if(found)
            {
                GameState.RecordMove(id, Choice);

                if (GameState.IsRoundComplete())
                {
                    GameState.GetWinnerId();
                }
            }
        }

        public async Task BroadcastGameStateAsync(string message)
        {
            foreach(var player in PlayerClients)
            {
                await player.Value.SendMessageAsync(message);
            }
        }

        public async Task EndGameAsync()
        {
            await BroadcastGameStateAsync("Game Over");

            foreach(var player in PlayerClients)
            {
                await player.Value.CloseConnectionAsync();
            }
        }

        public ICollection<string> GetPlayerIds()
        {
            return PlayerClients.Keys;
        }
    }
}
