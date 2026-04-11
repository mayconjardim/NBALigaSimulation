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
        public async Task<ActionResult<ServiceResponse<List<TradeDto>>>> GetTradeByTeamId()
        {
            var result = await _tradeService.GetTradesByTeamId();
            return result.Success ? Ok(result) : BadRequest(result);
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
        public async Task<ActionResult<ServiceResponse<TradeDto>>> CreateTrade(TradeCreateDto tradeDto)
        {
            var response = await _tradeService.CreateTrade(tradeDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteTrade(int id)
        {
            var response = await _tradeService.DeleteTrade(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }


        [HttpPut("update")]
        public async Task<ActionResult<ServiceResponse<bool>>> UpdateTrade(TradeDto dto)
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
