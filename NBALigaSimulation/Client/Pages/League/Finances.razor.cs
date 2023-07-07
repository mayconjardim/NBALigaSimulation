namespace NBALigaSimulation.Client.Pages.League
{
    partial class Finances
    {

        private List<TeamSimpleWithPlayersDto> teams;

        private string message = string.Empty;

        string[] headings = { "TEAM", "PAYROLL", "SPACE" };

        private int capSpace = 100000000;

        protected override async Task OnInitializedAsync()
        {
            message = "Carregando Time...";

            var result = await TeamService.GetAllTeamsWithPlayers();
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                teams = result.Data;
            }
        }


    }
}
