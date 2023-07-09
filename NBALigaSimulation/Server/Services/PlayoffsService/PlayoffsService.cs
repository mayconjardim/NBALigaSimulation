using AutoMapper;

namespace NBALigaSimulation.Server.Services.PlayoffsService
{
    public class PlayoffsService : IPlayoffsService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public PlayoffsService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<PlayoffsDto>> GetPlayoffs()
        {
            var response = new ServiceResponse<PlayoffsDto>();
            var season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();

            var playoffs = await _context.Playoffs.Where(p => p.Season == season.Year).ToListAsync();

            if (playoffs == null)
            {
                response.Success = false;
                response.Message = $"Playoffs não econtrado!";
            }
            else
            {
                response.Data = _mapper.Map<PlayoffsDto>(playoffs);
            }

            return response;
        }
    }
}
