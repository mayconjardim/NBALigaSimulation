using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NBALigaSimulation.Shared.Dtos.FA;
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
        public async Task<ActionResult<ServiceResponse<bool>>> CreateOffer(FAOfferDto offerDto)
        {
            var response = await _faService.CreateOffer(offerDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("teams")]
        public async Task<ActionResult<ServiceResponse<List<FAOfferDto>>>> GetOffersByTeamId()
        {
            var result = await _faService.GetOffersByTeamId();
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpDelete("{offerId}")]
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteOffer(int offerId)
        {
            var response = await _faService.DeleteOffer(offerId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("simulate-round")]
        public async Task<ActionResult<ServiceResponse<FASimulateRoundResultDto>>> SimulateFARound([FromQuery] int? season)
        {
            var result = await _faService.SimulateFARound(season);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
