using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{playoffsId}")]
        public async Task<ActionResult<ServiceResponse<PlayoffsDto>>> GetPlayerById(int playoffsId)
        {

            var result = await _playoffsService.GetPlayoffsById(playoffsId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

        [HttpPost("generate")]
        public async Task<ActionResult<ServiceResponse<bool>>> GeneratePlayoffs()
        {
            var result = await _playoffsService.GeneratePlayoffs();
            return Ok(result);
        }

        [HttpPost("generate/confsemis")]
        public async Task<ActionResult<ServiceResponse<bool>>> Generate2Round()
        {
            var result = await _playoffsService.Generate2Round();
            return Ok(result);
        }

        [HttpPost("generate/conffinals")]
        public async Task<ActionResult<ServiceResponse<bool>>> Generate3Round()
        {
            var result = await _playoffsService.Generate3Round();
            return Ok(result);
        }

        [HttpPost("generate/finals")]
        public async Task<ActionResult<ServiceResponse<bool>>> Generate4Round()
        {
            var result = await _playoffsService.Generate4Round();
            return Ok(result);
        }

        [HttpPost("end")]
        public async Task<ActionResult<ServiceResponse<bool>>> EndPlayoffs()
        {
            var result = await _playoffsService.EndPlayoffs();
            return Ok(result);
        }


    }
}
