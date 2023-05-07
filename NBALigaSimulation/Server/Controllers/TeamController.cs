using Microsoft.AspNetCore.Mvc;

namespace NBALigaSimulation.Server.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<ActionResult<ServiceResponse<TeamSimpleDto>>> GetTeamById(int teamId)
        {

            var result = await _teamService.GetTeamById(teamId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

    }
}
