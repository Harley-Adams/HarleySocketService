using HarleySocketService.PaperScissorsRock.Wiremodels;
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

        GameStateEnum GameState;
        string WinnerId;

        public PaperScissorsRockGame(string playerOneId, string playerTwoId)
        {
            PlayerOne = new PaperScissorsRockPlayer() { Id = playerOneId };
            PlayerTwo = new PaperScissorsRockPlayer() { Id = playerTwoId };
            GameState = GameStateEnum.InProgress;
        }

        public string GetWinnerId()
        {
            return WinnerId;
        }

        public GameStateEnum GetCurrentGameState()
        {
            return GameState;
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
            WinnerId = null;
            GameState = GameStateEnum.Lobby;
        }

        public void RecordMove(string playerId, PlayerChoiceEnum choice)
        {
            if (playerId == PlayerOne.Id)
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

        public void UpdateGameState()
        {
            if(!PlayerOne.HasPicked || !PlayerTwo.HasPicked)
            {
                GameState = GameStateEnum.InProgress;
                return;
            }

            if(PlayerOne.Choice == PlayerTwo.Choice)
            {
                GameState = GameStateEnum.Complete;
                WinnerId = "Tie";
                return;
            }

            var didPlayerOneWin = DidPlayerWin(PlayerOne.Choice, PlayerTwo.Choice);
            var didPlayerTwoWin = DidPlayerWin(PlayerTwo.Choice, PlayerOne.Choice);

            if(didPlayerOneWin && !didPlayerTwoWin)
            {
                PlayerOne.Score++;
                WinnerId = PlayerOne.Id;
                GameState = GameStateEnum.Complete;
            }
            else if(didPlayerTwoWin && !didPlayerOneWin)
            {
                PlayerTwo.Score++;
                GameState = GameStateEnum.Complete;
                WinnerId = PlayerTwo.Id;
            }

            return;
        }

        public void EndGame()
        {
            GameState = GameStateEnum.Complete;
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
    }
}
