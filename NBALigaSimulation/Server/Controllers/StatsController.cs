using Microsoft.AspNetCore.Mvc;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Controllers
{
	[Route("api/stats")]
	[ApiController]
	public class StatsController : ControllerBase
	{

		private readonly IStatsService _statsService;

		public StatsController(IStatsService statsService)
		{
			_statsService = statsService;
		}

		[HttpGet("teams")]
		public async Task<ActionResult<ServiceResponse<List<TeamRegularStatsDto>>>> GetAllTeamRegularStats(int season, bool isAscending, string stat = null)
		{
			try
			{
				var result = await _statsService.GetAllTeamRegularStats(season, isAscending, stat);

				if (!result.Success)
				{
					return StatusCode(500, result);
				}

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new ServiceResponse<PageableResponse<TeamRegularStatsDto>> { Success = false, Message = ex.Message });
			}
		}

		[HttpGet("playoffs/teams")]
		public async Task<ActionResult<ServiceResponse<List<TeamPlayoffsStatsDto>>>> GetAllTeamPlayoffsStats()
		{
			var result = await _statsService.GetAllTeamPlayoffsStats();
			return Ok(result);
		}

		[HttpGet("teams/ranks")]
		public async Task<ActionResult<ServiceResponse<List<TeamRegularStatsDto>>>> GetAllTeamRegularStatsRank()
		{
			var result = await _statsService.GetAllTeamRegularStatsRank();
			return Ok(result);
		}

		[HttpGet("players/{playerId:int}/regular-stats")]
		public async Task<ActionResult<ServiceResponse<List<PlayerRegularStatsDto>>>> GetPlayerRegularStatsByPlayerId(int playerId)
		{
			try
			{
				var result = await _statsService.GetPlayerRegularStatsByPlayerId(playerId);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new ServiceResponse<List<PlayerRegularStatsDto>> { Success = false, Message = ex.Message });
			}
		}

		[HttpGet("players")]
		public async Task<ActionResult<ServiceResponse<PageableResponse<PlayerRegularStatsDto>>>> GetAllPlayerRegularStats(int page, int pageSize, int season, bool isAscending, string position, string stat = null)
		{
			try
			{
				var result = await _statsService.GetAllPlayerRegularStats(page, pageSize, season, isAscending, position, stat);

				if (!result.Success)
				{
					return StatusCode(500, result);
				}

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new ServiceResponse<PageableResponse<PlayerRegularStatsDto>> { Success = false, Message = ex.Message });
			}
		}


		[HttpGet("playoffs/players")]
		public async Task<ActionResult<ServiceResponse<List<PlayerRegularStatsDto>>>> GetAllPlayerPlayoffsStats()
		{
			var result = await _statsService.GetAllPlayerPlayoffsStats();
			return Ok(result);
		}

	}
}
