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
            var teams = await _context.TeamRegularStats.ToListAsync();
            var response = new ServiceResponse<List<TeamRegularStatsDto>>
            {
                Data = _mapper.Map<List<TeamRegularStatsDto>>(teams)
            };

            return response;
        }
    }
}
