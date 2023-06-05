using MudBlazor;

namespace NBALigaSimulation.Client.Pages.User
{
    partial class UserRegister
    {

        UserRegistration user = new UserRegistration();

        string Message = string.Empty;
        string messageCssClass = string.Empty;

        async Task HandleRegistration()
        {

            var result = await AuthService.Register(user);
            Message = result.Message;
            if (result.Success)
            {
                messageCssClass = "success";
                Snackbar.Add("Registrado com sucesso!", Severity.Success);

            }
            else
            {
                messageCssClass = "error";
                Snackbar.Add($"{Message}", Severity.Error);

            }

        }

    }
}
