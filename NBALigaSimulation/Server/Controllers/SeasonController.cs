using Microsoft.AspNetCore.Mvc;

namespace NBALigaSimulation.Server.Controllers
{
    [Route("api/seasons")]
    [ApiController]
    public class SeasonController : ControllerBase
    {

        private readonly ISeasonService _seasonService;

        public SeasonController(ISeasonService seasonService)
        {
            _seasonService = seasonService;
        }


        [HttpGet]
        public async Task<ActionResult<ServiceResponse<CompleteSeasonDto>>> GetLastSeason()
        {

            var result = await _seasonService.GetLastSeason();

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<CompleteSeasonDto>>> CreateSeason(CreateSeasonDto request)
        {
            return Ok(await _seasonService.CreateSeason(request));
        }

        [HttpPut("update/{seasonId}")]
        public async Task<ActionResult<ServiceResponse<CompleteSeasonDto>>> UpdateSeason(int seasonId)
        {
            ServiceResponse<CompleteSeasonDto> response = await _seasonService.UpdateSeason(seasonId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

    }
}
