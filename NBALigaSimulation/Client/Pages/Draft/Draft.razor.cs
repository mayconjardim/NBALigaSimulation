namespace NBALigaSimulation.Client.Pages.Draft
{
    partial class Draft
    {

        private string message;
        private List<DraftDto> drafts;

        string[] headings = { "ROUND", "PICK", "TEAM", "PLAYER" };

        protected override async Task OnInitializedAsync()
        {
            message = "Carregando DRAFT...";

            var result = await DraftService.GetLastDraft();
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                drafts = result.Data;
            }
        }

    }
}
