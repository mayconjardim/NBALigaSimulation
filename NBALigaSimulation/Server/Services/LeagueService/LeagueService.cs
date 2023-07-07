using AutoMapper;

namespace NBALigaSimulation.Server.Services.League
{
    public class LeagueService : ILeagueService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public LeagueService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<TeamDraftPickDto>>> GetAllDraftPicks()
        {
            var teams = await _context.TeamDraftPicks.ToListAsync();
            var response = new ServiceResponse<List<TeamDraftPickDto>>
            {
                Data = _mapper.Map<List<TeamDraftPickDto>>(teams)
            };

            return response;
        }

    }
}
