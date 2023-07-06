using Microsoft.AspNetCore.Components;
using System.Collections;

namespace NBALigaSimulation.Client.Pages.Team
{
    partial class TeamHeader
    {
        [Parameter]
        public TeamCompleteDto? team { get; set; }

        private string message;

        private string ppg;
        private int ppgRank;
        private string rpg;
        private int rpgRank;
        private string apg;
        private int apgRank;
        private string oppg;
        private int oppgRank;

        protected override async Task OnInitializedAsync()
        {
            var result = await StatsService.GetAllTeamRegularStatsRank();

            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                var teamStats = result.Data.FirstOrDefault(t => t.TeamId == team.Id);

                if (teamStats != null)
                {
                    ppg = teamStats.Ppg;
                    rpg = teamStats.Rpg;
                    apg = teamStats.Apg;
                    oppg = teamStats.Oppg;

                    var sortedStats = result.Data.ToList();
                    ppgRank = sortedStats.OrderByDescending(t => Convert.ToDouble(t.Ppg)).ToList().FindIndex(t => t.TeamId == team.Id) + 1;
                    rpgRank = sortedStats.OrderByDescending(t => Convert.ToDouble(t.Rpg)).ToList().FindIndex(t => t.TeamId == team.Id) + 1;
                    apgRank = sortedStats.OrderByDescending(t => Convert.ToDouble(t.Apg)).ToList().FindIndex(t => t.TeamId == team.Id) + 1;
                    oppgRank = sortedStats.OrderBy(t => Convert.ToDouble(t.Oppg)).ToList().FindIndex(t => t.TeamId == team.Id) + 1;
                }
            }
        }





    }
}
