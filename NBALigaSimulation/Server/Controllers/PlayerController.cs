using Microsoft.AspNetCore.Mvc;

namespace NBALigaSimulation.Server.Controllers
{
    [Route("api/players")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<Player>>>> GetAllPlayers()
        {

            var result = await _playerService.GetAllPlayers();
            return Ok(result);

        }

        [HttpGet("{playerId}")]
        public async Task<ActionResult<ServiceResponse<Player>>> GetPlayerById(int playerId)
        {

            var result = await _playerService.GetPlayerById(playerId);

            if (!result.Success)
            {
                return NotFound();
            }

            return Ok(result);

        }

    }
}
