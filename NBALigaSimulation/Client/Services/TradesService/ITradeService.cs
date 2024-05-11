using NBALigaSimulation.Shared.Dtos.Trades;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.TradesService;

public interface ITradeService
{
    
    Task<ServiceResponse<List<TradeDto>>> GetTradeByTeamId();
    Task<ServiceResponse<List<TradeDto>>> GetAllTrades();
    Task<ServiceResponse<TradeDto>> GetTradeById(int tradeId);
    Task<ServiceResponse<TradeDto>> CreateTrade(TradeCreateDto tradeDto);
    Task<ServiceResponse<bool>> DeleteTrade(int tradeId);
    Task<ServiceResponse<bool>> UpdateTrade(TradeDto dto);
    
}