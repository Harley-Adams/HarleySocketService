using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarleySocketService.PaperScissorsRock
{
    public class PaperScissorsRockPlayer
    {
        public string Id { get; set; }
        public int Score { get; set; }
        public PlayerChoiceEnum Choice { get; set; }
        public bool HasPicked { get; set; }
    }
}
