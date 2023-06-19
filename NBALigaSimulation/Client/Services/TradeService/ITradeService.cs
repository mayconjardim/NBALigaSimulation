namespace NBALigaSimulation.Client.Services.TradeService
{
    public interface ITradeService
    {

        Task<ServiceResponse<TradeDto>> GetTradeByTeamId(int teamId);
        Task<ServiceResponse<List<TradeDto>>> GetAllTrades();

    }
}
