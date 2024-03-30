using Microsoft.AspNetCore.Mvc;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Controllers
{
    [Route("api/players")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayersController(IPlayerService playerService)
        {
            _playerService = playerService;
        }
        
        [HttpGet("{playerId}")]
        public async Task<ActionResult<ServiceResponse<PlayerCompleteDto>>> GetPlayerById(int playerId)
        {
            try
            {
                var result = await _playerService.GetPlayerById(playerId);

                if (!result.Success)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (ArgumentException ex) 
            {
                return StatusCode(500, ex.Message); 
            }
        }
        
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<PlayerSimpleDto>>>> GetAllPlayers()
        {
            var result = await _playerService.GetAllPlayers();
            return Ok(result);
        }
        
        [HttpGet("simple")]
        public async Task<ActionResult<ServiceResponse<List<PlayerSimpleDto>>>> GetAllSimplePlayers()
        {
            var result = await _playerService.GetAllSimplePlayers();
            return Ok(result);
        }

        [HttpGet("fa")]
        public async Task<ActionResult<ServiceResponse<List<PlayerSimpleDto>>>> GetAllFAPlayers()
        {

            var result = await _playerService.GetAllFaPlayers();
            return Ok(result);

        }

        [HttpGet("draft")]
        public async Task<ActionResult<ServiceResponse<List<PlayerSimpleDto>>>> GetAllDraftPlayers()
        {

            var result = await _playerService.GetAllDraftPlayers();
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
