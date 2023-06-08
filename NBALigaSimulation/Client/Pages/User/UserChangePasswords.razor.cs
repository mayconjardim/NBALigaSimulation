namespace NBALigaSimulation.Client.Pages.User
{
    partial class UserChangePasswords
    {

        UserChangePassword request = new UserChangePassword();
        string message = string.Empty;

        private async Task ChangePassword()
        {
            var result = await AuthService.ChangePassword(request);
            message = result.Message;
        }

    }
}
