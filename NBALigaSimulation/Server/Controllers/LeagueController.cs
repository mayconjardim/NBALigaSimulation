using Microsoft.AspNetCore.Mvc;
using NBALigaSimulation.Server.Services.LeagueService;
using NBALigaSimulation.Shared.Dtos.League;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Controllers
{
    [Route("api/league")]
    [ApiController]
    public class LeagueController : ControllerBase
    {

        private readonly ILeagueService _leagueService;

        public LeagueController(ILeagueService leagueService)
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

        [HttpGet("history")]
        public async Task<ActionResult<ServiceResponse<List<SeasonHistoryDto>>>> GetSeasonHistory()
        {
            var result = await _leagueService.GetSeasonHistory();
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
