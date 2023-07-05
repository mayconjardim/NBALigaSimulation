using System.Globalization;

namespace NBALigaSimulation.Client.Shared.Components
{
    partial class Standings
    {

        private List<TeamRegularStatsDto> statsEast = new List<TeamRegularStatsDto>();
        private List<TeamRegularStatsDto> statsWest = new List<TeamRegularStatsDto>();

        private string message = string.Empty;

        string[] east = { "EAST", "W-L", "PCT", "PF", "PA", "DIFF", "STRK" };
        string[] west = { "WEST", "W-L", "PCT", "PF", "PA", "DIFF", "STRK" };




        protected override async Task OnInitializedAsync()
        {
            message = "Carregando Stats...";

            var result = await StatsService.GetAllTeamRegularStats();
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                statsEast = result.Data.OrderByDescending(t => t.Wins).Where(t => t.TeamConference == "East").ToList();
                statsWest = result.Data.OrderByDescending(t => t.Wins).Where(t => t.TeamConference == "West").ToList();
            }

        }

        private string GetFormattedDiff(string diff)
        {
            double diffValue = double.Parse(diff, CultureInfo.InvariantCulture);
            string formattedDiff = diffValue.ToString("0.0", CultureInfo.InvariantCulture);

            if (diffValue > 0)
            {
                formattedDiff = "+" + formattedDiff;
            }

            return formattedDiff;
        }

        private string GetDiffStyle(string diff)
        {
            double diffValue = double.Parse(diff, CultureInfo.InvariantCulture);

            string style = "";

            if (diffValue < 0)
            {
                style = "color: red";
            }
            else if (diffValue > 0)
            {
                style = "color: green";
                diff = "+" + diff; // Adiciona o símbolo "+" ao valor da diferença
            }

            return $"{style}; font-weight: bold";
        }



    }
}

