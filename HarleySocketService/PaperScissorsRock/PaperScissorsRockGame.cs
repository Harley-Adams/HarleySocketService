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

        public PaperScissorsRockGame(string playerOneId, string playerTwoId)
        {
            PlayerOne = new PaperScissorsRockPlayer() { Id = playerOneId };
            PlayerTwo = new PaperScissorsRockPlayer() { Id = playerTwoId };
        }

        public void RecordMove(string playerId, PlayerChoiceEnum choice)
        {
            if(playerId == PlayerOne.Id)
            {
                PlayerOne.Choice = choice;
                PlayerOne.HasPicked = true;
            }
            else if (playerId == PlayerTwo.Id)
            {
                PlayerTwo.Choice = choice;
                PlayerTwo.HasPicked = true;
            }
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
                return "Winner: " + PlayerOne.Id;
            }
            else if(didPlayerTwoWin && !didPlayerOneWin)
            {
                return "Winner: " + PlayerTwo.Id;
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

        public Dictionary<string, int> GetScores()
        {
            return new Dictionary<string, int>()
            {
                {PlayerOne.Id, PlayerOne.Score },
                {PlayerTwo.Id, PlayerTwo.Score }
            };
        }

        public void ResetGameState()
        {
            PlayerOne.HasPicked = false;
            PlayerTwo.HasPicked = false;
        }
    }
}
