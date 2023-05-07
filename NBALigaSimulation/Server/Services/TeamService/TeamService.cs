using AutoMapper;

namespace NBALigaSimulation.Server.Services.TeamService
{
    public class TeamService : ITeamService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public TeamService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<TeamSimpleDto>>> GetAllTeams()
        {
            var teams = await _context.Teams.ToListAsync();
            var response = new ServiceResponse<List<TeamSimpleDto>>
            {
                Data = _mapper.Map<List<TeamSimpleDto>>(teams)
            };

            return response;
        }

        public async Task<ServiceResponse<TeamSimpleDto>> GetTeamById(int teamId)
        {
            var response = new ServiceResponse<TeamSimpleDto>();
            var team = await _context.Teams.FirstOrDefaultAsync(p => p.Id == teamId);

            if (team == null)
            {
                response.Success = false;
                response.Message = $"O Time com o Id {teamId} não existe!";
            }
            else
            {

                response.Data = _mapper.Map<TeamSimpleDto>(team);
            }

            return response;
        }

    }
}
