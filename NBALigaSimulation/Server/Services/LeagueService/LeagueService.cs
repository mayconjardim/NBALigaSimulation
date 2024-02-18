using AutoMapper;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.LeagueService
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
            var picks = await _context.TeamDraftPicks.Include(t => t.Team).ToListAsync();
            var response = new ServiceResponse<List<TeamDraftPickDto>>
            {
                Data = _mapper.Map<List<TeamDraftPickDto>>(picks)
            };

            return response;
        }

    }
}
