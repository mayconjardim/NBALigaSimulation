using Microsoft.AspNetCore.Mvc;

namespace NBALigaSimulation.Server.Controllers
{
    [Route("api/games")]
    [ApiController]
    public class GameController : ControllerBase
    {

        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }


        [HttpPost]
        public async Task<ActionResult<ServiceResponse<bool>>> CreateGame(CreateGameDto request)
        {
            return Ok(await _gameService.CreateGame(request));
        }

        [HttpPut("update/{gameId}")]
        public async Task<IActionResult> UpdateGame(int gameId)
        {
            ServiceResponse<bool> response = await _gameService.UpdateGame(gameId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<GameCompleteDto>>>> GetAllGames()
        {

            var result = await _gameService.GetAllGames();
            return Ok(result);

        }

        [HttpGet("{gameId}")]
        public async Task<ActionResult<ServiceResponse<GameCompleteDto>>> GetGameById(int gameId)
        {

            var result = await _gameService.GetGameById(gameId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

    }
}
