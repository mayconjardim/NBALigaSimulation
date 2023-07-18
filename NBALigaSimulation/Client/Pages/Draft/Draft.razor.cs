using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Client.Pages.Draft
{
    partial class Draft
    {

        private string message;
        private List<DraftDto> drafts;
        private int UserTeamId = 0;
        private DraftDto nextPick = null;

        string[] headings = { "ROUND", "PICK", "TEAM", "PLAYER" };

        private bool onTheClockShown = true;

        protected override async Task OnInitializedAsync()
        {
            message = "Carregando DRAFT...";

            string teamIdString = await LocalStorage.GetItemAsync<string>("teamId");

            if (!string.IsNullOrEmpty(teamIdString) && int.TryParse(teamIdString, out var userTeamId))
            {
                UserTeamId = userTeamId;
            }

            var result = await DraftService.GetLastDraft();
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                drafts = result.Data;
                nextPick = drafts.FirstOrDefault(draft => draft.PlayerId == null);
            }

        }

        private string getOnClock(int TeamId, int DTeamId)
        {
            if (TeamId == DTeamId)
            {
                onTheClockShown = false;
                return "ON THE CLOCK!";
            }
            return "";
        }


    }
}
