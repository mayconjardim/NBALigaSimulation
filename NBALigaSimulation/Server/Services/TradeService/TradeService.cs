using AutoMapper;
using Microsoft.EntityFrameworkCore;

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

            List<Trade> trades = await _context.Trades
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
                response.Data = _mapper.Map<TradeDto>(trade);
            }

            return response;
        }

        public async Task<ServiceResponse<TradeDto>> CreateTrade(TradeCreateDto tradeDto)
        {
            var response = new ServiceResponse<TradeDto>();

            try
            {

                Trade trade = _mapper.Map<Trade>(tradeDto);

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

            if (trade == null)
            {
                response.Success = false;
                response.Message = "Trade não encontrado.";
                return response;
            }

            trade.TradePlayers.Clear(); // Limpa os jogadores existentes da negociação

            List<TradePlayer> tradePlayers = _mapper.Map<List<TradePlayer>>(dto.TradePlayers);
            trade.TradePlayers.AddRange(tradePlayers); // Adiciona os jogadores mapeados

            _mapper.Map(dto, trade);

            try
            {
                await _context.SaveChangesAsync();
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


    }
}
