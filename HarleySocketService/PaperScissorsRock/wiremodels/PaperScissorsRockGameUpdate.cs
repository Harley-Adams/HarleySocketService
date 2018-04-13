﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarleySocketService.PaperScissorsRock.wiremodels
{
    public class PaperScissorsRockGameUpdate
    {
        public long timeStamp { get; set; }
        public string gameState { get; set; }
        public Dictionary<string, int> scores { get; set; }
    }
}