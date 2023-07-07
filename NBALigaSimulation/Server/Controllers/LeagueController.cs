using Microsoft.AspNetCore.Mvc;
using NBALigaSimulation.Server.Services.League;

namespace NBALigaSimulation.Server.Controllers
{
    [Route("api/league")]
    [ApiController]
    public class LeagueController : ControllerBase
    {

        private readonly ILeagueService _leagueService;

        public LeagueController(LeagueService leagueService)
        {
            _leagueService = leagueService;
        }

        [HttpGet("picks")]
        public async Task<ActionResult<ServiceResponse<TeamDraftPickDto>>> GetAllDraftPicks()
        {

            var result = await _leagueService.GetAllDraftPicks();

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

    }
}
