using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Client.Pages.FA
{
    partial class FAOffer
    {

        private string message = string.Empty;
        private int capSpace = 100000000;
        private int contract = 0;
        private int years = 1;
        private int season { get; set; }

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

                season = int.Parse(await LocalStorage.GetItemAsync<string>("season"));

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

        private int GetMinContract(int age)
        {
            switch (age)
            {
                case var _ when age < 21:
                    return 900000;
                case var _ when age < 24:
                    return 1500000;
                case var _ when age < 27:
                    return 1800000;
                case var _ when age < 30:
                    return 2000000;
                default:
                    return 2500000;
            }
        }

        private int GetMaxContract(int age)
        {
            switch (age)
            {
                case var _ when age < 26:
                    return 25000000;
                case var _ when age < 30:
                    return 30000000;
                default:
                    return 35000000;
            }
        }

        private bool HasCap(int contract)
        {

            if ((GetTeamTotalSalary() + contract) > capSpace)
            {
                return true;
            }

            return false;
        }




    }
}
