using Microsoft.AspNetCore.Mvc;

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


		[HttpPost("lottery")]
		public async Task<ActionResult<ServiceResponse<bool>>> GenerateLottery()
		{
			var result = await _draftService.GenerateLottery();
			return Ok(result);
		}


	}
}
