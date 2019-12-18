using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarleySocketService
{
    public class PlayfabManager
    {
        public async Task LoginAsync(string playerId)
        {
            PlayFabSettings.staticSettings.TitleId = "66E2"; // Please change this value to your own titleId from PlayFab Game Manager

            var request = new LoginWithCustomIDRequest { CustomId = playerId, CreateAccount = true };
            var loginTask = await PlayFabClientAPI.LoginWithCustomIDAsync(request);
            // If you want a synchronous ressult, you can call loginTask.Wait() - Note, this will halt the program until the function returns

            OnLoginComplete(loginTask);


            Console.WriteLine("Done! Press any key to close");
            Console.ReadKey(); // This halts the program and waits for the user
        }

        private void OnLoginComplete(PlayFabResult<LoginResult> taskResult)
        {
            var apiError = taskResult.Error;
            var apiResult = taskResult.Result;

            if (apiError != null)
            {
                Console.ForegroundColor = ConsoleColor.Red; // Make the error more visible
                Console.WriteLine("Something went wrong with your first API call.  :(");
                Console.WriteLine("Here's some debug information:");
                Console.WriteLine(PlayFabUtil.GenerateErrorReport(apiError));
                Console.ForegroundColor = ConsoleColor.Gray; // Reset to normal
            }
            else if (apiResult != null)
            {
                Console.WriteLine("Congratulations, you made your first successful API call!");
            }
        }
    }
}
