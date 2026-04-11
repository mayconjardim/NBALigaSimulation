using NBALigaSimulation.Shared.Dtos.Trades;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Features.Trades.Contracts;

public interface ITradeProposalValidator
{
    Task<ServiceResponse<bool>> ValidateCreateTrade(TradeCreateDto tradeDto);
}
