namespace NBALigaSimulation.Client.Pages.User
{
    partial class UserAdminProfile
    {

        private string message = string.Empty;
        string messageCssClass = string.Empty;

        private async Task CreateSeason()
        {

            var faResponse = await SeasonService.CreateSeason();

            if (faResponse.Success)
            {
                messageCssClass = "success";
                Snackbar.Add("Season criada com sucesso!", MudBlazor.Severity.Success);
            }
            else
            {
                messageCssClass = "error";
                Snackbar.Add("Season não criada!", MudBlazor.Severity.Error);

            }

        }

    }
}
