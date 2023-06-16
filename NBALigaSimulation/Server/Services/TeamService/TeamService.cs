using AutoMapper;

namespace NBALigaSimulation.Server.Services.TeamService
{
    public class TeamService : ITeamService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public TeamService(DataContext context, IMapper mapper, IAuthService authService)
        {
            _context = context;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<ServiceResponse<List<TeamSimpleDto>>> GetAllTeams()
        {
            var teams = await _context.Teams.Where(p => p.Players.Count > 0).ToListAsync();
            var response = new ServiceResponse<List<TeamSimpleDto>>
            {
                Data = _mapper.Map<List<TeamSimpleDto>>(teams)
            };

            return response;
        }

        public async Task<ServiceResponse<TeamCompleteDto>> GetTeamById(int teamId)
        {
            var response = new ServiceResponse<TeamCompleteDto>();
            var team = await _context.Teams
                 .Include(t => t.Players)
                     .ThenInclude(p => p.Ratings).Include(t => t.Gameplan)
                 .FirstOrDefaultAsync(p => p.Id == teamId);

            if (team == null)
            {
                response.Success = false;
                response.Message = $"O Time com o Id {teamId} não existe!";
            }
            else
            {
                response.Data = _mapper.Map<TeamCompleteDto>(team);
            }

            return response;
        }

        public async Task<ServiceResponse<TeamCompleteDto>> GetTeamByUser()
        {

            var response = new ServiceResponse<TeamCompleteDto>();
            var userId = _authService.GetUserId();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                response.Success = false;
                response.Message = "Usuário não encontrado!";
                return response;
            }

            var teamId = user.TeamId;

            if (teamId == null)
            {
                response.Success = false;
                response.Message = "Usuário não está associado a um time!";
                return response;
            }

            var team = await _context.Teams
            .Include(t => t.Players.OrderBy(p => p.RosterOrder))
                .ThenInclude(p => p.Ratings).Include(t => t.Gameplan)
            .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
            {
                response.Success = false;
                response.Message = $"O Time com o Id {teamId} não existe!";
                return response;
            }

            response.Data = _mapper.Map<TeamCompleteDto>(team);
            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateTeamGameplan(TeamGameplanDto teamGameplanDto)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            var teamGamePlan = await _context.TeamGameplans.FirstOrDefaultAsync(t => t.Id == teamGameplanDto.Id && t.TeamId == teamGameplanDto.TeamId);

            if (teamGamePlan == null)
            {
                teamGamePlan = new TeamGameplan();
                _mapper.Map(teamGameplanDto, teamGamePlan);
                _context.Add(teamGamePlan);
            }
            else
            {
                _mapper.Map(teamGameplanDto, teamGamePlan);
                _context.Update(teamGamePlan);
            }

            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "TeamGameplan updated successfully.";

            return response;
        }

    }
}
