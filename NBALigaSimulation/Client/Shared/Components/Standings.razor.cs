using System.Globalization;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Shared.Components;

public partial class Standings
{

    private List<TeamRegularStatsDto> statsEast;
    private List<TeamRegularStatsDto> statsWest;

        private string message = string.Empty;

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
                statsEast = result.Data.OrderByDescending(t => t.WinPct).Where(t => t.TeamConference == "East").ToList();
                statsWest = result.Data.OrderByDescending(t => t.WinPct).Where(t => t.TeamConference == "West").ToList();
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
                diff = "+" + diff; 
            }

            return $"{style}; font-weight: bold";
        }

        private string GetStreak(int streak)
        {

            if (streak > 0)
            {
                return "W" + streak;
            }
            else if (streak < 0)
            {
                return "L" + Math.Abs(streak);
            }
            else
            {
                return "L1";
            }

        }
    
}