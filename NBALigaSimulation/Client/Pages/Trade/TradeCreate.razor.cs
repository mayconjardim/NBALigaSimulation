using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace NBALigaSimulation.Client.Pages.Trade
{
    partial class TradeCreate
    {

        private int capSpace = 100000;
        private TeamCompleteDto? teamOne = null;
        private TeamCompleteDto? teamTwo = null;
        private List<TeamSimpleDto> teams = new List<TeamSimpleDto>();

        private string message = string.Empty;
        string messageCssClass = string.Empty;

        private List<PlayerCompleteDto> teamOneSend = new List<PlayerCompleteDto>();
        private List<PlayerCompleteDto> teamTwoSend = new List<PlayerCompleteDto>();

        private List<TeamDraftPickDto> teamOneSendPicks = new List<TeamDraftPickDto>();
        private List<TeamDraftPickDto> teamTwoSendPicks = new List<TeamDraftPickDto>();

        string[] headings = { "", "NAME", "POS", "AGE", "OVR", "POT", "CONTRACT" };


        protected override async Task OnInitializedAsync()
        {
            message = "Carregando Time...";

            var result = await TeamService.GetUserTeam();
            if (!result.Success)
            {

                message = result.Message;
            }
            else
            {
                teamOne = result.Data;

            }

            var results = await TeamService.GetAllTeams();
            if (!results.Success)
            {
                message = results.Message;
            }
            else
            {
                teams = results.Data.ToList();
                teams.RemoveAll(team => team.Id == teamOne.Id);
            }
        }

        private async Task OnTeamTwoSelected(IEnumerable<int> value)
        {

            int id = value.LastOrDefault();

            message = "Carregando Time...";
            var result = await TeamService.GetTeamById(id);
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                teamTwo = result.Data;
                teamOneSend.Clear();
                teamOneSendPicks.Clear();
                teamTwoSend.Clear();
                teamTwoSendPicks.Clear();
            }
        }

        private bool IsPlayerSelectedTeamOne(int playerId)
        {
            return teamOneSend.Any(p => p.Id == playerId);
        }

        private bool IsPickSelectedTeamOne(int pickId)
        {
            return teamOneSendPicks.Any(p => p.Id == pickId);
        }

        private void TogglePlayerSelectionTeamOne(int playerId)
        {
            if (teamOneSend.Any(p => p.Id == playerId))
            {
                var player = teamOneSend.Find(Player => Player.Id == playerId);
                teamOneSend.Remove(player);
            }
            else
            {
                var player = teamOne.Players.Find(Player => Player.Id == playerId);
                teamOneSend.Add(player);
            }
        }

        private void TogglePickSelectionTeamOne(int pickId)
        {
            if (teamOneSendPicks.Any(p => p.Id == pickId))
            {
                var pick = teamOneSendPicks.Find(Pick => Pick.Id == pickId);
                teamOneSendPicks.Remove(pick);
            }
            else
            {
                var pick = teamOne.DraftPicks.Find(Pick => Pick.Id == pickId);
                teamOneSendPicks.Add(pick);
            }
        }


        private decimal GetTeamOneTotalSalary()
        {
            return teamOneSend.Sum(p => p.Contract.Amount);
        }

        private bool IsPlayerSelectedTeamTwo(int playerId)
        {
            return teamTwoSend.Any(p => p.Id == playerId);
        }

        private bool IsPickSelectedTeamTwo(int pickId)
        {
            return teamTwoSendPicks.Any(p => p.Id == pickId);
        }

        private void TogglePlayerSelectionTeamTwo(int playerId)
        {
            if (teamTwoSend.Any(p => p.Id == playerId))
            {
                var player = teamTwoSend.Find(Player => Player.Id == playerId);
                teamTwoSend.Remove(player);
            }
            else
            {
                var player = teamTwo.Players.Find(Player => Player.Id == playerId);
                teamTwoSend.Add(player);
            }
        }

        private void TogglePickSelectionTeamTwo(int pickId)
        {
            if (teamTwoSendPicks.Any(p => p.Id == pickId))
            {
                var pick = teamTwoSendPicks.Find(Player => Player.Id == pickId);
                teamTwoSendPicks.Remove(pick);
            }
            else
            {
                var pick = teamTwo.DraftPicks.Find(picks => picks.Id == pickId);
                teamTwoSendPicks.Add(pick);
            }
        }

        private decimal GetTeamTwoSendTotalSalary()
        {
            return teamTwoSend.Sum(p => p.Contract.Amount);
        }

        private async Task SendTradeOffer()
        {
            var tradeOffer = new TradeCreateDto
            {
                TeamOneId = teamOne.Id,
                TeamTwoId = teamTwo.Id,
                TradePlayers = new List<TradePlayerDto>(),
                Players = new List<PlayerCompleteDto>(),
                DraftPicks = new List<TeamDraftPickDto>()
            };

            foreach (var player in teamOneSend)
            {
                tradeOffer.Players.Add(player);
            }

            foreach (var player in teamTwoSend)
            {
                tradeOffer.Players.Add(player);
            }

            foreach (var picks in teamOneSendPicks)
            {
                tradeOffer.DraftPicks.Add(picks);
            }

            foreach (var picks in teamTwoSendPicks)
            {
                tradeOffer.DraftPicks.Add(picks);
            }

            var tradeResponse = await TradeService.CreateTrade(tradeOffer);

            if (tradeResponse.Success)
            {
                messageCssClass = "success";
                Snackbar.Add("Proposta enviada com sucesso!", Severity.Success);
                NavigationManager.NavigateTo("/trades");
            }
            else
            {
                messageCssClass = "error";
                Snackbar.Add("Proposta não foi enviada!", Severity.Success);

            }

        }

    }

}

