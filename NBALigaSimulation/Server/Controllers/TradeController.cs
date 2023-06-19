using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NBALigaSimulation.Server.Services.TradeService;

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

        [HttpGet("{teamId}")]
        public async Task<ActionResult<ServiceResponse<TradeDto>>> GetTradeByTeamId(int teamId)
        {

            var result = await _tradeService.GetTradeByTeamId(teamId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

    }
}
