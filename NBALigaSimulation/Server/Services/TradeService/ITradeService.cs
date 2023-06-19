namespace NBALigaSimulation.Server.Services.TradeService
{
    public interface ITradeService
    {

        Task<ServiceResponse<List<TradeDto>>> GetTradesByTeamId();
        Task<ServiceResponse<TradeDto>> GetTradeById(int tradeId);
        Task<ServiceResponse<List<TradeDto>>> GetAllTrades();
        Task<ServiceResponse<TradeDto>> CreateTrade(TradeDto tradeDto);

    }
}
