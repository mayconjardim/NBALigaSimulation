using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NBALigaSimulation.Server.Services.StatsService;

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

    }
}
