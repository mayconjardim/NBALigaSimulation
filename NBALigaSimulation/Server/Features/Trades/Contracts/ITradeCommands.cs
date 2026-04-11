using NBALigaSimulation.Shared.Dtos.Trades;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Features.Trades.Contracts;

public interface ITradeCommands
{
    Task<ServiceResponse<TradeDto>> CreateTrade(TradeCreateDto tradeDto);
    Task<ServiceResponse<bool>> UpdateTrade(TradeDto dto);
    Task<ServiceResponse<bool>> DeleteTrade(int tradeId);
}
