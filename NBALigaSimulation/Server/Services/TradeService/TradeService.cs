using AutoMapper;

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

        public async Task<ServiceResponse<List<TradeDto>>> GetTradeByTeamId()
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

    }
}
