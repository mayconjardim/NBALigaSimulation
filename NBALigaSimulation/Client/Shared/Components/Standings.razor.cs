using System.Globalization;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Shared.Components;

public partial class Standings
{

    private List<TeamRegularStatsDto> statsEast;
    private List<TeamRegularStatsDto> statsWest;
    private int _season = 0;
    private bool isAscending = false;
    
        private string message = string.Empty;

        protected override async Task OnInitializedAsync()
        {

             if (!(await LocalStorage.ContainKeyAsync("season")))
            {
               await LocalStorage.SetItemAsync("season", _season);;
            }

            _season = int.Parse(await LocalStorage.GetItemAsync<string>("season"));

            message = "Carregando Stats...";

            var result = await StatsService.GetAllTeamRegularStats(_season, isAscending, null);
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                statsEast = result.Data.Where(t => t.TeamConference == "East").ToList();
                statsWest = result.Data.Where(t => t.TeamConference == "West").ToList();
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

        private string GetDiffClass(string diff)
        {
            double diffValue = double.Parse(diff, CultureInfo.InvariantCulture);

            if (diffValue < 0)
            {
                return "diff-negative";
            }
            else if (diffValue > 0)
            {
                return "diff-positive";
            }
            else
            {
                return "diff-neutral";
            }
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

        private string GetStreakClass(int streak)
        {
            if (streak > 0)
            {
                return "streak-win";
            }
            else
            {
                return "streak-loss";
            }
        }
    
}
