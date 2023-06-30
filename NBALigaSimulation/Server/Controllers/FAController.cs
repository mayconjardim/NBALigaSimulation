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

    }

}
