using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }

}
