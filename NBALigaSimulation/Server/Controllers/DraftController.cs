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


    }
}
