using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Client.Pages.User
{
    partial class UserLogins
    {

        private string errorMessage = string.Empty;

        private string returnUrl = string.Empty;

        private UserLogin user = new();


        async Task HandleLogin()
        {
            await Console.Out.WriteLineAsync("VASCO");
        }
    }
}
