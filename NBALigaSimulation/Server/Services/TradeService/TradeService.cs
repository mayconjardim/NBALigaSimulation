using AutoMapper;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Client.Pages.Player;
using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Server.Services.TradeService
{
    public class TradeService : ITradeService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public TradeService(DataContext context, IMapper mapper, IAuthService authService)
        {
            _context = context;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<ServiceResponse<List<TradeDto>>> GetAllTrades()
        {
            var trades = await _context.Trades.Include(t => t.TeamOne).Include(t => t.TeamTwo).ToListAsync();
            var response = new ServiceResponse<List<TradeDto>>
            {
                Data = _mapper.Map<List<TradeDto>>(trades)
            };

            return response;
        }

        public async Task<ServiceResponse<List<TradeDto>>> GetTradesByTeamId()
        {
            var response = new ServiceResponse<List<TradeDto>>();

            var userId = _authService.GetUserId();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            int? teamId = null;

            if (user != null)
            {
                teamId = user.TeamId;
            }

            List<Trade> trades = await _context.Trades.OrderByDescending(o => o.DateCreated)
                    .Where(p => p.TeamOneId == teamId || p.TeamTwoId == teamId).Include(t => t.TeamOne).Include(t => t.TeamTwo)
                    .ToListAsync();

            if (trades == null)
            {
                response.Success = false;
                response.Message = $"A trade com time de Id {teamId} não existe!";
            }
            else
            {
                response.Data = _mapper.Map<List<TradeDto>>(trades);
            }

            return response;
        }

        public async Task<ServiceResponse<TradeDto>> GetTradeById(int tradeId)
        {
            var response = new ServiceResponse<TradeDto>();

            var trade = await _context.Trades
                .Include(t => t.TeamOne)
                .Include(t => t.TeamTwo)
                .Include(t => t.TradePlayers)
                .FirstOrDefaultAsync(t => t.Id == tradeId);

            if (trade == null)
            {
                response.Success = false;
                response.Message = $"A trade com Id {tradeId} não existe!";
            }
            else
            {
                var tradeWithPlayers = _mapper.Map<TradeDto>(trade);
                tradeWithPlayers.Players = new List<PlayerCompleteDto>();

                if (trade.TradePlayers != null)
                {
                    foreach (var t in trade.TradePlayers)
                    {
                        var player = await _context.Players.Include(p => p.Ratings).Include(p => p.Contract).FirstOrDefaultAsync(p => p.Id == t.PlayerId);
                        var playerDto = _mapper.Map<PlayerCompleteDto>(player);
                        tradeWithPlayers.Players.Add(playerDto);
                    }
                }

                response.Data = tradeWithPlayers;
            }

            return response;
        }

        public async Task<ServiceResponse<TradeDto>> CreateTrade(TradeCreateDto tradeDto)
        {
            var response = new ServiceResponse<TradeDto>();

            try
            {

                Trade trade = _mapper.Map<Trade>(tradeDto);

                foreach (var player in tradeDto.Players)
                {
                    trade.TradePlayers.Add(new TradePlayer
                    {
                        PlayerId = player.Id,
                        TradePlayerId = trade.Id
                    });
                }

                _context.Trades.Add(trade);
                await _context.SaveChangesAsync();
                response.Success = true;
                response.Data = _mapper.Map<TradeDto>(trade);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocorreu um erro ao criar a trade: " + ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateTrade(TradeDto dto)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            Trade trade = await _context.Trades.Include(t => t.TradePlayers)
                .FirstOrDefaultAsync(p => p.Id == dto.Id);

            var TeamOne = await _context.Teams.Include(t => t.Players).FirstOrDefaultAsync(t => t.Id == dto.TeamOneId);
            var TeamTwo = await _context.Teams.Include(t => t.Players).FirstOrDefaultAsync(t => t.Id == dto.TeamTwoId);

            if (trade == null)
            {
                response.Success = false;
                response.Message = "Trade não encontrado.";
                return response;
            }

            if (dto.Response == true)
            {
                trade.Response = true;

                foreach (var player in trade.TradePlayers)
                {
                    Random random = new Random();
                    int randomNumber = random.Next(15, 1000);
                    var playerDb = await _context.Players.FirstOrDefaultAsync(p => p.Id == player.PlayerId);
                    if (playerDb.TeamId == trade.TeamOneId)
                    {
                        playerDb.TeamId = trade.TeamTwoId;
                        playerDb.PtModifier = 1;
                        playerDb.RosterOrder = TeamTwo.Players.Count + randomNumber;

                    }
                    else if (playerDb.TeamId == trade.TeamTwoId)
                    {
                        playerDb.TeamId = trade.TeamOneId;
                        playerDb.PtModifier = 1;
                        playerDb.RosterOrder = TeamOne.Players.Count + randomNumber;
                    }

                }

            }

            if (dto.Response == false)
            {
                trade.Response = false;
            }

            try
            {
                await _context.SaveChangesAsync();
                UpdateRosterOrderAfterTrade(TeamOne.Id, TeamTwo.Id);
                response.Success = true;
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Erro ao atualizar o Trade.";
            }

            return response;
        }


        public async Task<ServiceResponse<bool>> UpdateRosterOrderAfterTrade(int teamOneId, int teamTwoId)
        {
            var response = new ServiceResponse<bool>();

            var teamOneList = await _context.Players.OrderBy(p => p.RosterOrder).Where(p => p.TeamId == teamOneId).ToListAsync();
            var teamTwoList = await _context.Players.OrderBy(p => p.RosterOrder).Where(p => p.TeamId == teamTwoId).ToListAsync();


            foreach (var player in teamOneList)
            {
                var playerDb = await _context.Players.FirstOrDefaultAsync(p => p.Id == player.Id);

                if (playerDb == null)
                {
                    response.Success = false;
                    response.Message = $"O Player com o Id {player.Id} não existe!";
                    return response;

                }
                else
                {
                    int rosterOrder = teamOneList.IndexOf(player);
                    playerDb.RosterOrder = rosterOrder;
                }

            }

            foreach (var player in teamTwoList)
            {
                var playerDb = await _context.Players.FirstOrDefaultAsync(p => p.Id == player.Id);

                if (playerDb == null)
                {
                    response.Success = false;
                    response.Message = $"O Player com o Id {player.Id} não existe!";
                    return response;

                }
                else
                {
                    int rosterOrder = teamTwoList.IndexOf(player);
                    playerDb.RosterOrder = rosterOrder;
                }

            }

            await _context.SaveChangesAsync();
            response.Success = true;
            response.Message = "Roster order updated successfully.";

            return response;
        }

    }
}
