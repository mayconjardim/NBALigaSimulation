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

        [HttpGet("fa")]
        public async Task<ActionResult<ServiceResponse<List<PlayerSimpleDto>>>> GetAllFAPlayers()
        {

            var result = await _playerService.GetAllFAPlayers();
            return Ok(result);

        }

        [HttpGet("draft")]
        public async Task<ActionResult<ServiceResponse<List<PlayerSimpleDto>>>> GetAllDraftPlayers()
        {

            var result = await _playerService.GetAllDraftPlayers();
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
        public async Task<ActionResult> UpdatePlayerPtModifier(int playerId, [FromBody] decimal newPtModifier)
        {
            var response = await _playerService.UpdatePlayerPtModifier(playerId, newPtModifier);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("contracts")]
        public async Task<ActionResult> GenerateContracts()
        {
            var response = await _playerService.GenerateContracts();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("searchsuggestion/{searchText}")]
        public async Task<ActionResult<ServiceResponse<List<PlayerSimpleDto>>>> GetPlayersSearchSuggestions(string searchText)
        {

            var result = await _playerService.GetPlayersSearchSuggestions(searchText);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

    }
}
