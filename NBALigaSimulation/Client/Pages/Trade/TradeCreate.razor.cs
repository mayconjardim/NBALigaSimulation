namespace NBALigaSimulation.Client.Pages.Trade
{
    partial class TradeCreate
    {

        private int capSpace = 100000;
        private TradeDto trade = null;
        private TeamCompleteDto? teamOne = null;
        private TeamCompleteDto? teamTwo = null;
        private List<TeamSimpleDto> teams = new List<TeamSimpleDto>();
        private string message = string.Empty;
        private List<PlayerCompleteDto> teamOneSend = new List<PlayerCompleteDto>();
        private List<PlayerCompleteDto> teamTwoSend = new List<PlayerCompleteDto>();

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
                teamTwoSend.Clear();
            }
        }

        private bool IsPlayerSelectedTeamOne(int playerId)
        {
            return teamOneSend.Any(p => p.Id == playerId);
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

        private decimal GetTeamOneTotalSalary()
        {
            return teamOneSend.Sum(p => p.Contract.Amount);
        }

        private bool IsPlayerSelectedTeamTwo(int playerId)
        {
            return teamTwoSend.Any(p => p.Id == playerId);
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
            };

            var tradeResponse = await TradeService.CreateTrade(tradeOffer);

            if (tradeResponse.Success)
            {
                TradeDto trade = tradeResponse.Data;

                trade.TradePlayers = new List<TradePlayerDto>();

                foreach (var player in teamOneSend)
                {
                    TradePlayerDto tradePlayer = new TradePlayerDto
                    {
                        TradePlayerId = trade.Id,
                        PlayerId = player.Id
                    };
                    trade.TradePlayers.Add(tradePlayer);
                }

                foreach (var player in teamTwoSend)
                {
                    TradePlayerDto tradePlayer = new TradePlayerDto
                    {
                        TradePlayerId = trade.Id,
                        PlayerId = player.Id
                    };
                    trade.TradePlayers.Add(tradePlayer);
                }

                foreach (var player in trade.TradePlayers)
                {
                    await Console.Out.WriteLineAsync(player.PlayerId.ToString());
                }


            }

        }

    }
}
