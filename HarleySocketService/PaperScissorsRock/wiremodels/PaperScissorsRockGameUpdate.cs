using HarleySocketService.Models;
using System.Collections.Generic;

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
