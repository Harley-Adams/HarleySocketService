using HarleySocketService.PaperScissorsRock.Wiremodels;
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
        public PaperScissorsRockPlayerChoiceEnum Choice { get; set; }
        public bool HasPicked { get; set; }
    }
}
