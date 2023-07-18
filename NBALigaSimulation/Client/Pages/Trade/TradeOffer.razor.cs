using Microsoft.AspNetCore.Components;
using MudBlazor;
using NBALigaSimulation.Shared.Dtos;

namespace NBALigaSimulation.Client.Pages.Trade
{
    partial class TradeOffer
    {

        string[] headings = { "", "NAME", "POS", "AGE", "OVR", "POT", "CONTRACT" };

        private TradeDto? trade = null;
        private string message = string.Empty;
        string messageCssClass = string.Empty;
        private string userTeam { get; set; } = string.Empty;
        private List<PlayerCompleteDto> teamOnePlayers = new List<PlayerCompleteDto>();
        private List<TeamDraftPickDto> teamOnePicks = new List<TeamDraftPickDto>();

        private List<PlayerCompleteDto> teamTwoPlayers = new List<PlayerCompleteDto>();
        private List<TeamDraftPickDto> teamTwoPicks = new List<TeamDraftPickDto>();


        [Parameter]
        public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            message = "Carregando Time...";

            var result = await TradeService.GetTradeById(Id);
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {

                userTeam = await LocalStorage.GetItemAsync<string>("team");
                trade = result.Data;

                foreach (var player in trade.Players)
                {

                    if (player.TeamId == trade.TeamOneId)
                    {
                        teamOnePlayers.Add(player);
                    }

                    if (player.TeamId == trade.TeamTwoId)
                    {
                        teamTwoPlayers.Add(player);
                    }
                }

                foreach (var pick in trade.DraftPicks)
                {

                    if (pick.TeamId == trade.TeamOneId)
                    {
                        teamOnePicks.Add(pick);
                    }

                    if (pick.TeamId == trade.TeamTwoId)
                    {
                        teamTwoPicks.Add(pick);
                    }
                }

            }
        }

        private decimal GetTeamOneTotalSalary()
        {
            return teamOnePlayers.Sum(p => p.Contract.Amount);
        }

        private decimal GetTeamTwoTotalSalary()
        {
            return teamTwoPlayers.Sum(p => p.Contract.Amount);
        }

        private async Task AcceptTrade()
        {
            trade.Response = true;
            var sucesss = await TradeService.UpdateTrade(trade);

            if (sucesss.Success)
            {
                messageCssClass = "success";
                Snackbar.Add("Proposta respondida com sucesso!", Severity.Success);
                NavigationManager.NavigateTo("/trades");
            }
            else
            {
                messageCssClass = "error";
                Snackbar.Add("Proposta não foi enviada!", Severity.Error);

            }

        }

        private async Task DeclineTrade()
        {
            trade.Response = false;
            var sucesss = await TradeService.UpdateTrade(trade);

            if (sucesss.Success)
            {
                messageCssClass = "success";
                Snackbar.Add("Proposta respondida com sucesso!", Severity.Success);
                NavigationManager.NavigateTo("/trades");
            }
            else
            {
                messageCssClass = "error";
                Snackbar.Add("Proposta não foi enviada!", Severity.Error);

            }

        }

    }
}
