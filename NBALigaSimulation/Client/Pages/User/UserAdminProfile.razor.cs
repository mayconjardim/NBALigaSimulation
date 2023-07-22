namespace NBALigaSimulation.Client.Pages.User
{
    partial class UserAdminProfile
    {

        private string message = string.Empty;
        string messageCssClass = string.Empty;

        private async Task CreateSeason()
        {

            var seasonResponse = await SeasonService.CreateSeason();

            if (seasonResponse.Success)
            {
                messageCssClass = "success";
                Snackbar.Add(seasonResponse.Message, MudBlazor.Severity.Success);
            }
            else
            {
                messageCssClass = "error";
                Snackbar.Add(seasonResponse.Message, MudBlazor.Severity.Error);

            }

        }

        private async Task CreateLottery()
        {

            var draftResponse = await DraftService.GenerateLottery();

            if (draftResponse.Success)
            {
                messageCssClass = "success";
                Snackbar.Add(draftResponse.Message, MudBlazor.Severity.Success);
            }
            else
            {
                messageCssClass = "error";
                Snackbar.Add(draftResponse.Message, MudBlazor.Severity.Error);

            }

        }

    }
}
