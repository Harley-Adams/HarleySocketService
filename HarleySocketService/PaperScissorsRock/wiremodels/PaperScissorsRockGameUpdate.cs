using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarleySocketService.PaperScissorsRock.Wiremodels
{
    public class PaperScissorsRockGameUpdate
    {
        public long timeStamp { get; set; }
        public GameStateEnum gameState { get; set; }
        public string winnerId { get; set; }
        public Dictionary<string, int> scores { get; set; }
    }
}
