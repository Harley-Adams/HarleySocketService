using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarleySocketService.PaperScissorsRock.Wiremodels
{
    public enum GameStateEnum
    {
        NotInGame,
        Lobby,
        InProgress,
        Complete
    }
}
