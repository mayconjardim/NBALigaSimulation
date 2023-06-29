using Microsoft.AspNetCore.Components;

namespace NBALigaSimulation.Client.Pages.FA
{
    partial class FAOffer
    {

        private string message = string.Empty;
        private int capSpace = 100000000;

        [Parameter]
        public int Id { get; set; }

        private PlayerCompleteDto? player = null;
        private TeamCompleteDto? team = null;

        protected override async Task OnParametersSetAsync()
        {
            message = "Carregando Jogador...";

            var result = await PlayerService.GetPlayerById(Id);
            var teamResult = await TeamService.GetUserTeam();

            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {

                if (result.Data.TeamId == 21)
                {
                    team = teamResult.Data;
                    player = result.Data;
                }
                else
                {
                    message = "Não é possivel enviar proposta pra esse jogador!";
                }

            }

        }

        private decimal GetTeamTotalSalary()
        {
            return team.Players.Sum(p => p.Contract.Amount);
        }

    }
}
