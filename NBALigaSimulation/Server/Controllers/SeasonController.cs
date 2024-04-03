using Microsoft.AspNetCore.Mvc;
using NBALigaSimulation.Shared.Dtos.Seasons;
using NBALigaSimulation.Shared.Models.Utils;
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

        [HttpGet("all")]
        [SwaggerOperation(Summary = "GetLastSeason", Description = "Retorna todas as temporadas.")]
        public async Task<ActionResult<ServiceResponse<List<CompleteSeasonDto>>>> GetALlSeason()
        {

            var result = await _seasonService.GetALlSeason();

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet]
        [SwaggerOperation(Summary = "GetAllSeason", Description = "Retorna a temporada atual.")]
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
        [SwaggerOperation(Summary = "CreateSeason", Description = "Cria uma nova temporada.")]

        public async Task<ActionResult<ServiceResponse<CompleteSeasonDto>>> CreateSeason()
        {
            return Ok(await _seasonService.CreateSeason());
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

        [HttpDelete("clean-schedule")]
        public async Task<ActionResult<ServiceResponse<CompleteSeasonDto>>> CleanSchedule()
        {
            {
                ServiceResponse<CompleteSeasonDto> response = await _seasonService.CleanSchedule();

                if (!response.Success)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
        }
    }
}
