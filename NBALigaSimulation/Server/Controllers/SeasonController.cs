using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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

        [HttpPost]
        [SwaggerOperation(Summary = "CreateSeason", Description = "Cria uma nova temporada.")]

        public async Task<ActionResult<ServiceResponse<CompleteSeasonDto>>> CreateSeason()
        {
            return Ok(await _seasonService.CreateSeason());
        }

        [HttpGet]
        [SwaggerOperation(Summary = "GetLastSeason", Description = "Retorna a temporada atual.")]
        public async Task<ActionResult<ServiceResponse<CompleteSeasonDto>>> GetLastSeason()
        {

            var result = await _seasonService.GetLastSeason();

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

        [HttpPut("schedule")]
        [SwaggerOperation(Summary = "GenerateSchedule", Description = "Gera os jogos da temporada regular.")]
        public async Task<ActionResult<ServiceResponse<CompleteSeasonDto>>> GenerateSchedule()
        {
            ServiceResponse<CompleteSeasonDto> response = await _seasonService.GenerateSchedule();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("tc")]
        [SwaggerOperation(Summary = "GenerateTrainingCamp", Description = "Realiza a evolução/regressão dos jogadores.")]
        public async Task<ActionResult<ServiceResponse<CompleteSeasonDto>>> GenerateTrainingCamp()
        {
            ServiceResponse<CompleteSeasonDto> response = await _seasonService.GenerateTrainingCamp();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

    }
}
