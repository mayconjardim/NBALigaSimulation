using Microsoft.AspNetCore.Mvc;

namespace NBALigaSimulation.Server.Controllers
{
    [Route("api/stats")]
    [ApiController]
    public class StatsController : ControllerBase
    {

        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        [HttpGet("teams")]
        public async Task<ActionResult<ServiceResponse<List<TeamRegularStatsDto>>>> GetAllTeamRegularStats()
        {
            var result = await _statsService.GetAllTeamRegularStats();
            return Ok(result);
        }

        [HttpGet("players/{playerId}")]
        public async Task<ActionResult<ServiceResponse<List<TeamRegularStatsDto>>>> GetAllRegularStatsByPlayerId(int playerId)
        {
            var result = await _statsService.GetAllRegularStatsByPlayerId(playerId);
            return Ok(result);
        }

    }
}
