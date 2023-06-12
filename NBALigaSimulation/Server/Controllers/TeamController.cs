using Microsoft.AspNetCore.Mvc;

namespace NBALigaSimulation.Server.Controllers
{
    [Route("api/teams")]
    [ApiController]
    public class TeamController : ControllerBase
    {

        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<TeamSimpleDto>>>> GetAllTeams()
        {

            var result = await _teamService.GetAllTeams();
            return Ok(result);

        }

        [HttpGet("{teamId}")]
        public async Task<ActionResult<ServiceResponse<TeamCompleteDto>>> GetTeamById(int teamId)
        {

            var result = await _teamService.GetTeamById(teamId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

        [HttpGet("profile")]
        public async Task<ActionResult<ServiceResponse<TeamCompleteDto>>> GetTeamByUser()
        {

            var result = await _teamService.GetTeamByUser();

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

    }
}
