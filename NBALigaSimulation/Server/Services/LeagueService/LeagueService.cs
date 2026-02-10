using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.LeagueService
{
    public class LeagueService : ILeagueService
    {

        private readonly IGenericRepository<TeamDraftPicks> _draftPickRepository;
        private readonly IMapper _mapper;

        public LeagueService(
            IGenericRepository<TeamDraftPicks> draftPickRepository,
            IMapper mapper)
        {
            _draftPickRepository = draftPickRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<TeamDraftPickDto>>> GetAllDraftPicks()
        {
            var picks = await _draftPickRepository.Query()
                .Include(t => t.Team)
                .ToListAsync();
            var response = new ServiceResponse<List<TeamDraftPickDto>>
            {
                Data = _mapper.Map<List<TeamDraftPickDto>>(picks)
            };

            return response;
        }

    }
}
