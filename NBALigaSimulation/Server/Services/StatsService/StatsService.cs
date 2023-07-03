using AutoMapper;

namespace NBALigaSimulation.Server.Services.StatsService
{
    public class StatsService : IStatsService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public StatsService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<TeamRegularStatsDto>>> GetAllTeamRegularStats()
        {

            var season = _context.Seasons.OrderBy(s => s.Year).Last();

            var teamRegularStatsList = await _context.TeamRegularStats
                .Where(t => t.Season == season.Year)
                .Include(t => t.Team)
                .ToListAsync();

            var response = new ServiceResponse<List<TeamRegularStatsDto>>
            {
                Data = _mapper.Map<List<TeamRegularStatsDto>>(teamRegularStatsList)
            };

            return response;
        }

    }
}
