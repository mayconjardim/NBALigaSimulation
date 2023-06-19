namespace NBALigaSimulation.Server.Services.TradeService
{
    public interface ITradeService
    {

        Task<ServiceResponse<List<TradeDto>>> GetTradeByTeamId();
        Task<ServiceResponse<List<TradeDto>>> GetAllTrades();

    }
}
