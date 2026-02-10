using Microsoft.AspNetCore.Mvc;
using NBALigaSimulation.Shared.Dtos.Games;
using NBALigaSimulation.Shared.Models.Utils;

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
		public async Task<ActionResult<ServiceResponse<GameCompleteDto>>> CreateGame(CreateGameDto request)
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

		[HttpPut("update/sim")]
		public async Task<IActionResult> UpdateGames()
		{
			ServiceResponse<bool> response = await _gameService.UpdateGames();

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

		[HttpGet("teams/{teamId}")]
		public async Task<ActionResult<ServiceResponse<List<GameCompleteDto>>>> GetGamesByTeamId(int teamId)
		{
			try
			{
				var result = await _gameService.GetGamesByTeamId(teamId);

				if (!result.Success)
				{
					return NotFound(result);
				}

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Ocorreu um erro ao buscar os jogos para o Time com o ID {teamId}: {ex.Message}");
			}
		}

		[HttpPut("update/date/playoffs")]
		public async Task<ActionResult<ServiceResponse<bool>>> SimGameByDatePlayoffs()
		{
			ServiceResponse<bool> response = await _gameService.SimGameByDatePlayoffs();

			if (!response.Success)
			{
				return BadRequest(response);
			}

			return Ok(response);
		}

		[HttpPut("update/date/regular")]
		public async Task<ActionResult<ServiceResponse<bool>>> SimGameByDateRegular()
		{
			ServiceResponse<bool> response = await _gameService.SimGameByDateRegular();

			if (!response.Success)
			{
				return BadRequest(response);
			}

			return Ok(response);
		}

		[HttpPut("update/round/{roundNumber}")]
		public async Task<ActionResult<ServiceResponse<bool>>> SimGameByRound(int roundNumber)
		{
			ServiceResponse<bool> response = await _gameService.SimGameByRound(roundNumber);

			if (!response.Success)
			{
				return BadRequest(response);
			}

			return Ok(response);
		}

		[HttpPut("update/playoffs/round/{playoffRound}")]
		public async Task<ActionResult<ServiceResponse<bool>>> SimPlayoffsByRound(int playoffRound)
		{
			ServiceResponse<bool> response = await _gameService.SimPlayoffsByRound(playoffRound);

			if (!response.Success)
			{
				return BadRequest(response);
			}

			return Ok(response);
		}

		[HttpPut("update/date/all")]
		public async Task<ActionResult<ServiceResponse<bool>>> SimAll()
		{
			ServiceResponse<bool> response = await _gameService.SimAll();

			if (!response.Success)
			{
				return BadRequest(response);
			}

			return Ok(response);
		}
	}
}
