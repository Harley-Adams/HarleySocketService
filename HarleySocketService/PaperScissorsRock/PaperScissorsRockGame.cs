using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarleySocketService.PaperScissorsRock
{
    public class PaperScissorsRockGame
    {
        PaperScissorsRockPlayer PlayerOne;
        PaperScissorsRockPlayer PlayerTwo;

        public void RecordMove(string playerId, PlayerChoiceEnum choice)
        {
            if(playerId == PlayerOne.Id)
            {
                PlayerOne.Choice = choice;
                PlayerOne.HasPicked = true;
                return;
            }
            else if (playerId == PlayerOne.Id)
            {
                PlayerTwo.Choice = choice;
                PlayerTwo.HasPicked = true;
                return;
            }
        }

        public bool IsRoundComplete()
        {
            return PlayerOne.HasPicked && PlayerTwo.HasPicked;
        }

        public string GetWinnerId()
        {
            if(!PlayerOne.HasPicked || !PlayerTwo.HasPicked)
            {
                return "InProgress";
            }

            if(PlayerOne.Choice == PlayerTwo.Choice)
            {
                return "Tie";
            }

            var didPlayerOneWin = DidPlayerWin(PlayerOne.Choice, PlayerTwo.Choice);
            var didPlayerTwoWin = DidPlayerWin(PlayerTwo.Choice, PlayerOne.Choice);

            if(didPlayerOneWin && !didPlayerTwoWin)
            {
                return PlayerOne.Id;
            }
            else if(didPlayerTwoWin && !didPlayerOneWin)
            {
                return PlayerTwo.Id;
            }

            return "Error";
        }

        private bool DidPlayerWin(PlayerChoiceEnum winChoice, PlayerChoiceEnum looseChoice)
        {
            if(winChoice == PlayerChoiceEnum.Paper && looseChoice == PlayerChoiceEnum.Rock)
            {
                return true;
            }
            if (winChoice == PlayerChoiceEnum.Rock && looseChoice == PlayerChoiceEnum.Scissors)
            {
                return true;
            }
            if (winChoice == PlayerChoiceEnum.Scissors && looseChoice == PlayerChoiceEnum.Paper)
            {
                return true;
            }

            return false;
        }

        public void ResetGameState()
        {
            PlayerOne.HasPicked = true;
            PlayerTwo.HasPicked = true;
        }
    }
}
