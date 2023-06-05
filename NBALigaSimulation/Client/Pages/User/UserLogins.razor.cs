using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Client.Pages.User
{
    partial class UserLogins
    {

        private string errorMessage = string.Empty;

        private UserLogin user = new();


        private async Task HandleLogin()
        {
            var result = await AuthService.Login(user);
            if (result.Success)
            {
                errorMessage = string.Empty;

                await LocalStorage.SetItemAsync("authToken", result.Data);
                NavigationManager.NavigateTo("");
            }
            else
            {
                errorMessage = result.Message;
            }
        }
    }
}
