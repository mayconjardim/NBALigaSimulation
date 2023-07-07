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
            var teams = await _context.Teams.Where(t => t.IsHuman == true).ToListAsync();
            var response = new ServiceResponse<List<TeamSimpleDto>>
            {
                Data = _mapper.Map<List<TeamSimpleDto>>(teams)
            };

            return response;
        }

        public async Task<ServiceResponse<List<TeamSimpleWithPlayersDto>>> GetAllTeamsWithPlayers()
        {
            var teams = await _context.Teams.Where(t => t.IsHuman == true).Include(t => t.Players).ThenInclude(p => p.Contract).ToListAsync();
            var response = new ServiceResponse<List<TeamSimpleWithPlayersDto>>
            {
                Data = _mapper.Map<List<TeamSimpleWithPlayersDto>>(teams)
            };

            return response;
        }

        public async Task<ServiceResponse<TeamCompleteDto>> GetTeamById(int teamId)
        {
            var response = new ServiceResponse<TeamCompleteDto>();
            var team = await _context.Teams
             .Include(t => t.Players)
                 .ThenInclude(p => p.Ratings)
             .Include(p => p.Players)
                 .ThenInclude(p => p.Contract)
             .Include(p => p.Players)
                 .ThenInclude(p => p.RegularStats)
             .Include(t => t.DraftPicks)
             .FirstOrDefaultAsync(t => t.Id == teamId);

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
                .Include(p => p.Players).ThenInclude(p => p.Contract)
                .Include(t => t.DraftPicks)
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

        public async Task<ServiceResponse<bool>> UpdateTeamGameplan(int teamId, TeamGameplanDto teamGameplanDto)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            var team = await _context.Teams.Include(t => t.Gameplan).FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
            {
                response.Message = "Team not found.";
                return response;
            }

            team.Gameplan.Pace = teamGameplanDto.Pace;
            team.Gameplan.Motion = teamGameplanDto.Motion;
            team.Gameplan.Focus = teamGameplanDto.Focus;
            team.Gameplan.Defense = teamGameplanDto.Defense;

            _context.Update(team);

            try
            {
                await _context.SaveChangesAsync();
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message = $"Error updating team gameplan: {ex.Message}";
            }

            return response;
        }

    }
}
