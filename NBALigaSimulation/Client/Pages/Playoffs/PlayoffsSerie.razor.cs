using Microsoft.AspNetCore.Components;

namespace NBALigaSimulation.Client.Pages.Playoffs
{
    partial class PlayoffsSerie
    {

        private PlayoffsDto? playoffs = null;
        private string message = string.Empty;

        [Parameter]
        public int Id { get; set; }

        string[] headings = { "HOME", "AWAY", "DATE", "RESULT" };

        protected override async Task OnParametersSetAsync()
        {
            message = "Carregando Serie...";

            var result = await PlayoffsService.GetPlayoffsById(Id);
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                playoffs = result.Data;
            }
        }



    }
}
