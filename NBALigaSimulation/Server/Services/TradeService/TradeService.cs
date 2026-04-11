using AutoMapper;
using NBALigaSimulation.Server.Features.Trades.Contracts;
using NBALigaSimulation.Shared.Dtos.Trades;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.TradeService
{
    public class TradeService : ITradeService
    {
        private readonly ITradeQueries _tradeQueries;
        private readonly ITradeCommands _tradeCommands;

        public TradeService(
            ITradeQueries tradeQueries,
            ITradeCommands tradeCommands)
        {
            _tradeQueries = tradeQueries;
            _tradeCommands = tradeCommands;
        }

        public Task<ServiceResponse<List<TradeDto>>> GetAllTrades() => _tradeQueries.GetAllTrades();
        public Task<ServiceResponse<List<TradeDto>>> GetTradesByTeamId() => _tradeQueries.GetTradesByTeamId();
        public Task<ServiceResponse<TradeDto>> GetTradeById(int tradeId) => _tradeQueries.GetTradeById(tradeId);
        public Task<ServiceResponse<TradeDto>> CreateTrade(TradeCreateDto tradeDto) => _tradeCommands.CreateTrade(tradeDto);
        public Task<ServiceResponse<bool>> UpdateTrade(TradeDto dto) => _tradeCommands.UpdateTrade(dto);
        public Task<ServiceResponse<bool>> DeleteTrade(int tradeId) => _tradeCommands.DeleteTrade(tradeId);
    }
}
