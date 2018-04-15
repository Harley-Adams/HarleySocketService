using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarleySocketService
{
    public enum PlayerConnectionStateEnum
    {
        Connected,
        Lobby,
        SearchingForGame,
        InGame,
        PostGame
    }
}
