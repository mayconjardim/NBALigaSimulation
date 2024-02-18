using NBALigaSimulation.Shared.Dtos.Trades;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.TradeService
{
    public interface ITradeService
    {

        Task<ServiceResponse<List<TradeDto>>> GetTradesByTeamId();
        Task<ServiceResponse<TradeDto>> GetTradeById(int tradeId);
        Task<ServiceResponse<List<TradeDto>>> GetAllTrades();
        Task<ServiceResponse<TradeDto>> CreateTrade(TradeCreateDto tradeDto);
        Task<ServiceResponse<bool>> UpdateTrade(TradeDto dto);
        Task<ServiceResponse<bool>> DeleteTrade(int tradeId);
    }
}
