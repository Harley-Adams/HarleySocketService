using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace HarleySocketService
{
    interface IGameInstance
    {
        void RecievePlayerUpdate(string id, string playerMessage);
        Task UpdatePlayersWithGameState();
        Task EndGameAsync();
        bool IsGameComplete();
        ICollection<string> GetPlayerIds();
    }
}
