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

        [HttpGet("players")]
        public async Task<ActionResult<ServiceResponse<List<TeamSimpleWithPlayersDto>>>> GetAllTeamsWithPlayers()
        {

            var result = await _teamService.GetAllTeamsWithPlayers();
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

        [HttpPut("{teamId}/gameplan")]
        public async Task<ActionResult> UpdateTeamGameplan(int teamId, TeamGameplanDto teamGameplanDto)
        {
            var response = await _teamService.UpdateTeamGameplan(teamId, teamGameplanDto);

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok();
        }

        [HttpPut("{teamId}/keys")]
        public async Task<ActionResult> UpdateKeyPlayers(int teamId, List<PlayerCompleteDto> players)
        {
            var response = await _teamService.UpdateKeyPlayers(teamId, players);

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok();
        }

    }
}
