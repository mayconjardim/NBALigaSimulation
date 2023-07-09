using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Server.Services.PlayoffsService;

namespace NBALigaSimulation.Server.Controllers
{
    [Route("api/playoffs")]
    [ApiController]
    public class PlayoffsController : ControllerBase
    {

        private readonly IPlayoffsService _playoffsService;

        public PlayoffsController(IPlayoffsService playoffsService)
        {
            _playoffsService = playoffsService;
        }


        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<PlayoffsDto>>>> GetPlayoffs()
        {
            var result = await _playoffsService.GetPlayoffs();
            return Ok(result);

        }

        [HttpPost("generate")]
        public async Task<ActionResult<ServiceResponse<bool>>> GeneratePlayoffs()
        {
            return Ok(await _playoffsService.GeneratePlayoffs());
        }


    }
}
