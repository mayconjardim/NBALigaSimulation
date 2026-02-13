using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NBALigaSimulation.Shared.Dtos.FA;
using NBALigaSimulation.Shared.Dtos.Trades;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Controllers
{
    [Route("api/fa")]
    [ApiController]
    public class FAController : ControllerBase
    {

        private readonly IFAService _faService;

        public FAController(IFAService faService)
        {
            _faService = faService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateOffer(FAOfferDto offerDto)
        {
            var response = await _faService.CreateOffer(offerDto);

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Data);
        }

        [HttpGet("teams")]
        public async Task<ActionResult<ServiceResponse<TradeDto>>> GetOffersByTeamId()
        {

            var result = await _faService.GetOffersByTeamId();

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

        [HttpDelete("{offerId}")]
        public async Task<ActionResult> DeleteOffer(int offerId)
        {
            var response = await _faService.DeleteOffer(offerId);

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok();
        }

        [HttpPost("simulate-round")]
        public async Task<ActionResult<ServiceResponse<FASimulateRoundResultDto>>> SimulateFARound([FromQuery] int? season)
        {
            var result = await _faService.SimulateFARound(season);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result);
        }
    }
}
