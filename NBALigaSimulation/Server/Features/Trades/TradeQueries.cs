using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Server.Features.Trades.Contracts;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Dtos.Trades;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Teams;
using NBALigaSimulation.Shared.Models.Trades;
using NBALigaSimulation.Shared.Models.Users;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Features.Trades;

public class TradeQueries : ITradeQueries
{
    private readonly IGenericRepository<Trade> _tradeRepository;
    private readonly IGenericRepository<Player> _playerRepository;
    private readonly IGenericRepository<TeamDraftPicks> _draftPickRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;

    public TradeQueries(
        IGenericRepository<Trade> tradeRepository,
        IGenericRepository<Player> playerRepository,
        IGenericRepository<TeamDraftPicks> draftPickRepository,
        IGenericRepository<User> userRepository,
        IMapper mapper,
        IAuthService authService)
    {
        _tradeRepository = tradeRepository;
        _playerRepository = playerRepository;
        _draftPickRepository = draftPickRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _authService = authService;
    }

    public async Task<ServiceResponse<List<TradeDto>>> GetAllTrades()
    {
        var trades = await _tradeRepository.Query()
            .Include(t => t.TeamOne)
            .Include(t => t.TeamTwo)
            .ToListAsync();

        return new ServiceResponse<List<TradeDto>>
        {
            Data = _mapper.Map<List<TradeDto>>(trades)
        };
    }

    public async Task<ServiceResponse<List<TradeDto>>> GetTradesByTeamId()
    {
        var response = new ServiceResponse<List<TradeDto>>();

        try
        {
            int? teamId = null;
            try
            {
                var userId = _authService.GetUserId();
                var user = await _userRepository.Query()
                    .FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null)
                {
                    teamId = user.TeamId;
                }
            }
            catch
            {
                teamId = null;
            }

            List<Trade> trades = await _tradeRepository.Query()
                .OrderByDescending(o => o.DateCreated)
                .Where(p => p.TeamOneId == teamId || p.TeamTwoId == teamId)
                .Include(t => t.TeamOne)
                .Include(t => t.TeamTwo)
                .ToListAsync();

            response.Data = trades != null ? _mapper.Map<List<TradeDto>>(trades) : new List<TradeDto>();
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Erro ao buscar trades: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<TradeDto>> GetTradeById(int tradeId)
    {
        var response = new ServiceResponse<TradeDto>();

        var trade = await _tradeRepository.Query()
            .Include(t => t.TeamOne)
            .Include(t => t.TeamTwo)
            .Include(t => t.TradePlayers)
            .Include(t => t.TradePicks)
            .FirstOrDefaultAsync(t => t.Id == tradeId);

        if (trade == null)
        {
            response.Success = false;
            response.Message = $"A trade com Id {tradeId} não existe!";
            return response;
        }

        var tradeWithPlayers = _mapper.Map<TradeDto>(trade);
        tradeWithPlayers.Players = new List<PlayerCompleteDto>();
        tradeWithPlayers.DraftPicks = new List<TeamDraftPickDto>();

        var tradePlayerIds = trade.TradePlayers?
            .Select(t => t.PlayerId)
            .Distinct()
            .ToList() ?? new List<int>();

        if (tradePlayerIds.Count > 0)
        {
            var players = await _playerRepository.Query()
                .Include(p => p.Ratings)
                .Include(p => p.Contract)
                .Where(p => tradePlayerIds.Contains(p.Id))
                .ToListAsync();

            var playerMap = players.ToDictionary(p => p.Id);
            tradeWithPlayers.Players = trade.TradePlayers!
                .Where(t => playerMap.ContainsKey(t.PlayerId))
                .Select(t => _mapper.Map<PlayerCompleteDto>(playerMap[t.PlayerId]))
                .ToList();
        }

        var tradePickIds = trade.TradePicks?
            .Select(t => t.DraftPickId)
            .Distinct()
            .ToList() ?? new List<int>();

        if (tradePickIds.Count > 0)
        {
            var picks = await _draftPickRepository.Query()
                .Where(p => tradePickIds.Contains(p.Id))
                .ToListAsync();

            var pickMap = picks.ToDictionary(p => p.Id);
            tradeWithPlayers.DraftPicks = trade.TradePicks!
                .Where(t => pickMap.ContainsKey(t.DraftPickId))
                .Select(t => _mapper.Map<TeamDraftPickDto>(pickMap[t.DraftPickId]))
                .ToList();
        }

        response.Data = tradeWithPlayers;
        return response;
    }
}
