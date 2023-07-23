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

        private async Task GenerateTC()
        {

            var seasonResponse = await SeasonService.GenerateTrainingCamp();

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

        private async Task GenerateSchedule()
        {

            var seasonResponse = await SeasonService.GenerateSchedule();

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

        private async Task SimGameByDateRegular()
        {
            var gameResponse = await GameService.SimGameByDateRegular();

            if (gameResponse.Success)
            {
                messageCssClass = "success";
                Snackbar.Add(gameResponse.Message, MudBlazor.Severity.Success);
            }
            else
            {
                messageCssClass = "error";
                Snackbar.Add(gameResponse.Message, MudBlazor.Severity.Error);

            }
        }

        private async Task SimGameByDatePlayoffs()
        {
            var gameResponse = await GameService.SimGameByDatePlayoffs();

            if (gameResponse.Success)
            {
                messageCssClass = "success";
                Snackbar.Add(gameResponse.Message, MudBlazor.Severity.Success);
            }
            else
            {
                messageCssClass = "error";
                Snackbar.Add(gameResponse.Message, MudBlazor.Severity.Error);

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

        private async Task CreateDraft()
        {

            var draftResponse = await DraftService.GenerateDraft();

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
