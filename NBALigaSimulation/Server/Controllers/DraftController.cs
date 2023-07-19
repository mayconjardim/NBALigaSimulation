using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace NBALigaSimulation.Server.Controllers
{
    [Route("api/draft")]
    [ApiController]
    public class DraftController : ControllerBase
    {

        private readonly IDraftService _draftService;

        public DraftController(IDraftService draftService)
        {
            _draftService = draftService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "GetLastDraft", Description = "Retorna a ultimo draft realizado.")]
        public async Task<ActionResult<ServiceResponse<List<DraftDto>>>> GetLastDraft()
        {

            var result = await _draftService.GetLastDraft();

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

        [HttpGet("lotto")]
        [SwaggerOperation(Summary = "GetLastLottery", Description = "Retorna a ultima loteria realizada.")]
        public async Task<ActionResult<ServiceResponse<DraftLotteryDto>>> GetLastLottery()
        {

            var result = await _draftService.GetLastLottery();

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

        [HttpPost("generate/lottery")]
        public async Task<ActionResult<ServiceResponse<bool>>> GenerateLottery()
        {
            var result = await _draftService.GenerateLottery();
            return Ok(result);
        }

        [HttpPost("generate/draft")]
        public async Task<ActionResult<ServiceResponse<bool>>> GenerateDraft()
        {
            var result = await _draftService.GenerateDraft();
            return Ok(result);
        }

        [HttpPut("select")]
        public async Task<ActionResult<ServiceResponse<bool>>> SelectDraftedPlayer(DraftPlayerDto request)
        {
            var response = await _draftService.SelectDraftedPlayer(request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

    }
}
