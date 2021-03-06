﻿using HarleySocketService.Models;
using HarleySocketService.PaperScissorsRock;
using HarleySocketService.PaperScissorsRock.Wiremodels;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace HarleySocketService
{
    public class GameInstance : IGameInstance
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

        public void RecievePlayerUpdate(string id, string playerMessage)
        {
            PlayerClient client;
            var found = PlayerClients.TryGetValue(id, out client);

            if (found)
            {
                var playerChoice = JsonConvert.DeserializeObject<PaperScissorsRockPlayerMessage>(playerMessage);
                GameState.RecordMove(id, playerChoice.choice);
                GameState.UpdateGameState();
            }
        }

        public async Task UpdatePlayersWithGameState()
        {
            var gameStateUpdate = new PaperScissorsRockGameUpdate()
            {
                timeStamp = DateTime.UtcNow.Ticks,
                gameState = GameState.GetCurrentGameState(),
                scores = GameState.GetScores(),
                winnerId = GameState.GetWinnerId(),
            };

            await BroadcastMessageToPlayersAsync(JsonConvert.SerializeObject(gameStateUpdate));
        }

        public async Task EndGameAsync()
        {
            GameState.EndGame();

            await UpdatePlayersWithGameState();
        }

        public bool IsGameComplete()
        {
            return GameState.GetCurrentGameState() == GameStateEnum.Complete;
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
