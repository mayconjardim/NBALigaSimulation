using Microsoft.AspNetCore.Mvc;

namespace NBALigaSimulation.Server.Controllers
{
    [Route("api/trades")]
    [ApiController]
    public class TradeController : ControllerBase
    {

        private readonly ITradeService _tradeService;

        public TradeController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<TradeDto>>>> GetAllTrades()
        {

            var result = await _tradeService.GetAllTrades();
            return Ok(result);

        }

        [HttpGet("teams")]
        public async Task<ActionResult<ServiceResponse<TradeDto>>> GetTradeByTeamId()
        {

            var result = await _tradeService.GetTradeByTeamId();

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

    }
}
