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
            try
            {
                var result = await _awardsService.GetAllAwards();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponse<List<PlayerAwardsDto>> { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("generate/{season:int}")]
        public async Task<ActionResult<ServiceResponse<bool>>> GenerateAwards(int season)
        {
            try
            {
                var result = await _awardsService.GenerateAwards(season);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponse<bool> { Success = false, Message = ex.Message });
            }
        }
    }
}
