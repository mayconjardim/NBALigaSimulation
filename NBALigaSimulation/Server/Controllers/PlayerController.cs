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
        public async Task<ActionResult<ServiceResponse<List<PlayerSimpleDto>>>> GetAllPlayers()
        {

            var result = await _playerService.GetAllPlayers();
            return Ok(result);

        }

        [HttpGet("{playerId}")]
        public async Task<ActionResult<ServiceResponse<PlayerCompleteDto>>> GetPlayerById(int playerId)
        {

            var result = await _playerService.GetPlayerById(playerId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<PlayerCompleteDto>>> CreatePlayer(CreatePlayerDto request)
        {
            return Ok(await _playerService.CreatePlayer(request));
        }

        [HttpPost("multi")]
        public async Task<ActionResult<ServiceResponse<PlayerCompleteDto>>> CreatePlayers(List<CreatePlayerDto> requests)
        {
            return Ok(await _playerService.CreatePlayers(requests));
        }


        [HttpPut("rosterorder")]
        public async Task<ActionResult> UpdateRosterOrder(List<PlayerCompleteDto> updatedPlayerList)
        {
            var response = await _playerService.UpdateRosterOrder(updatedPlayerList);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("{playerId}/ptmodifier")]
        public async Task<ActionResult> UpdatePlayerPtModifier(int playerId, [FromBody] double newPtModifier)
        {
            var response = await _playerService.UpdatePlayerPtModifier(playerId, newPtModifier);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }


    }
}
