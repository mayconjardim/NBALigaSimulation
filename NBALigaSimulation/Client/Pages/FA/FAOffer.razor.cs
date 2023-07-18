using Microsoft.AspNetCore.Components;
using MudBlazor;
using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Client.Pages.FA
{
    partial class FAOffer
    {

        private int capSpace = 100000000;
        private int contract = 0;
        private int years = 1;

        private string message = string.Empty;
        string messageCssClass = string.Empty;

        private int season { get; set; }

        [Parameter]
        public int Id { get; set; }

        private PlayerCompleteDto? player = null;
        private TeamCompleteDto? team = null;

        protected override async Task OnInitializedAsync()
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

        public async Task MinContractAsync()
        {
            int minContract = GetMinContract((season - player.Born.Year));
            years = 1;
            contract = minContract;
            await SendFAOffer();
        }

        public async Task MaxContractAsync()
        {
            int maxContract = GetMaxContract((season - player.Born.Year));
            years = 4;
            contract = maxContract;
            await SendFAOffer();
        }

        public async Task PersonalizedContractAsync()
        {
            if (contract > 0 && years >= 1)
            {
                await SendFAOffer();
            }
        }

        private async Task SendFAOffer()
        {
            var faOffer = new FAOfferDto
            {
                TeamId = team.Id,
                PlayerName = player.Name,
                PlayerId = player.Id,
                Amount = contract,
                Years = years,
                Season = season
            };


            var faResponse = await FAService.CreateOffer(faOffer);

            if (faResponse.Success)
            {
                messageCssClass = "success";
                Snackbar.Add("Proposta enviada com sucesso!", Severity.Success);
                NavigationManager.NavigateTo("/freeagency");
            }
            else
            {
                messageCssClass = "error";
                Snackbar.Add("Proposta não foi enviada!", Severity.Success);

            }

        }


    }
}
