using Microsoft.AspNetCore.Mvc;
using NBALigaSimulation.Shared.Dtos.Trades;
using NBALigaSimulation.Shared.Models.Utils;

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

            var result = await _tradeService.GetTradesByTeamId();

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

        [HttpGet("{tradeId}")]
        public async Task<ActionResult<ServiceResponse<TradeDto>>> GetTradeById(int tradeId)
        {

            var result = await _tradeService.GetTradeById(tradeId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

        [HttpPost]
        public async Task<ActionResult> CreateTrade(TradeCreateDto tradeDto)
        {
            var response = await _tradeService.CreateTrade(tradeDto);

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTrade(int id)
        {
            var response = await _tradeService.DeleteTrade(id);

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok();
        }


        [HttpPut("update")]
        public async Task<IActionResult> UpdateTrade(TradeDto dto)
        {
            ServiceResponse<bool> response = await _tradeService.UpdateTrade(dto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
