using Microsoft.AspNetCore.Mvc;
using NBALigaSimulation.Server.Services.AwardsService;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Controllers
{
    [Route("api/awards")]
    [ApiController]
    public class AwardsController : ControllerBase
    {
        private readonly IAwardsService _awardsService;

        public AwardsController(IAwardsService awardsService)
        {
            _awardsService = awardsService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<PlayerAwardsDto>>>> GetAllAwards()
        {
            var result = await _awardsService.GetAllAwards();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("generate/{season:int}")]
        public async Task<ActionResult<ServiceResponse<bool>>> GenerateAwards(int season)
        {
            var result = await _awardsService.GenerateAwards(season);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
