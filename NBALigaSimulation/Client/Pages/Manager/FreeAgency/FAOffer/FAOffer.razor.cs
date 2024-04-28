using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.FA;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.Manager.FreeAgency.FAOffer;

public partial class FAOffer 
{
    
    [Parameter]
    public int Id { get; set; }
    
    private PlayerCompleteDto? player = null;
    private TeamCompleteDto? team = null;
    private int capSpace = 100000000;
    private int _contract = 0;
    private int _years = 1;
    private int _season;
    
    private string message = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        message = "Carregando Jogador...";

        _season = int.Parse(await LocalStorage.GetItemAsync<string>("season"));

        var result = await PlayerService.GetPlayerById(Id);
        var teamResult = await TeamService.GetTeamByUser();
        if (result.Success)
        {
            if (result.Data?.TeamId == 21)
            {
                team = teamResult.Data;
                player = result.Data;
            }
            else
            {
                message = "Não é possivel enviar proposta pra esse jogador!";
            }
        }
        else
        {
            message = result.Message;
        }
    }
    
      private decimal GetTeamTotalSalary()
        {
            if (team.Players.Any(p => p.Contract != null && p.Contract.Amount != null))
            {
                return team.Players.Where(p => p.Contract != null && p.Contract.Amount != null)
                    .Sum(p => p.Contract.Amount);
            }

            return 0;
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
            int minContract = GetMinContract((_season - player.Born.Year));
            _years = 1;
            _contract = minContract;
            await SendFAOffer();
        }

        public async Task MaxContractAsync()
        {
            int maxContract = GetMaxContract((_season - player.Born.Year));
            _years = 4;
            _contract = maxContract;
            await SendFAOffer();
        }

        public async Task PersonalizedContractAsync()
        {
            if (_contract > 0 && _years >= 1)
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
                Amount = _contract,
                Years = _years,
                Season = _season
            };


            var faResponse = await FAService.CreateOffer(faOffer);

            if (faResponse.Success)
            {
                //Snackbar.Add("Proposta enviada com sucesso!", Severity.Success);
                NavigationManager.NavigateTo("/freeagency");
            }
            else
            {
                //Snackbar.Add("Proposta não foi enviada!", Severity.Success);
            }

        }


}