using NBALigaSimulation.Shared.Dtos.Trades;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Features.Trades.Contracts;

public interface ITradeQueries
{
    Task<ServiceResponse<List<TradeDto>>> GetAllTrades();
    Task<ServiceResponse<List<TradeDto>>> GetTradesByTeamId();
    Task<ServiceResponse<TradeDto>> GetTradeById(int tradeId);
}
