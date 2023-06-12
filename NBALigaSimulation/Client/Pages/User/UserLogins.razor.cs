using Microsoft.AspNetCore.Components;

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
                await AuthenticationStateProvider.GetAuthenticationStateAsync();
                NavigationManager.NavigateTo("");
            }
            else
            {
                errorMessage = result.Message;
            }
        }
    }
}
