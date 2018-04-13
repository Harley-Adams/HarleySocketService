using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarleySocketService.PaperScissorsRock.wiremodels
{
    public class PaperScissorsRockGameChoice
    {
        public long timeStamp { get; set; }
        public PlayerChoiceEnum choice { get; set; }
    }
}
