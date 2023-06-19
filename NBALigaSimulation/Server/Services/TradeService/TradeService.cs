using AutoMapper;

namespace NBALigaSimulation.Server.Services.TradeService
{
    public class TradeService : ITradeService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public TradeService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<TradeDto>>> GetAllTrades()
        {
            var trades = await _context.Trades.ToListAsync();
            var response = new ServiceResponse<List<TradeDto>>
            {
                Data = _mapper.Map<List<TradeDto>>(trades)
            };

            return response;
        }

        public async Task<ServiceResponse<TradeDto>> GetTradeByTeamId(int teamId)
        {
            var response = new ServiceResponse<TradeDto>();
            var trade = await _context.Trades.FirstOrDefaultAsync(p => p.TeamOneId == teamId || p.TeamTwoId == teamId);


            if (trade == null)
            {
                response.Success = false;
                response.Message = $"A trade com time de Id {teamId} não existe!";
            }
            else
            {
                response.Data = _mapper.Map<TradeDto>(trade);
            }

            return response;
        }
    }
}
