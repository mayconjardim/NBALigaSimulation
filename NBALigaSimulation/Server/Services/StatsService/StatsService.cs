using AutoMapper;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.StatsService
{
	public class StatsService : IStatsService
	{

		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public StatsService(DataContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		
	public async Task<ServiceResponse<PlayerStatsResponse>> GetAllPlayerRegularStats(int page, int pageSize, int season, string stat = null)
	{
	    var response = new ServiceResponse<PlayerStatsResponse>();

	    try
	    {
	        IQueryable<PlayerRegularStats> query = _context.PlayerRegularStats;

	        var orderByExpression = query.OrderByDescending(p => (p.Pts / p.Games));

	        if (!string.IsNullOrEmpty(stat))
	        {
	            orderByExpression = stat switch
	            {
	                "GP" => query.OrderByDescending(p => p.Games),
	                "MIN" => query.OrderByDescending(p => (p.Min / p.Games)),
	                "FG%" => query.OrderByDescending(p => (p.Fg / p.Fga * 100)),
	                "3P%" => query.OrderByDescending(p => (p.Tp / p.Tpa * 100)),
	                "ORB" => query.OrderByDescending(p => (p.Orb / p.Games)),
	                "DRB" => query.OrderByDescending(p => (p.Drb / p.Games)),
	                "RPG" => query.OrderByDescending(p => (p.Trb / p.Games)),
	                "APG" => query.OrderByDescending(p => (p.Ast / p.Games)),
	                "SPG" => query.OrderByDescending(p => (p.Stl / p.Games)),
	                "BPG" => query.OrderByDescending(p => (p.Blk / p.Games)),
	                "TPG" => query.OrderByDescending(p => (p.Tov / p.Games)),
	                "TS%" => query.OrderByDescending(p => ((p.Pts / (2.0 * (p.Fga + (0.44 * p.Fta)))) * 100)),
	                _ => orderByExpression
	            };
	        }

	        query = orderByExpression.Where(s => s.Season == season && s.Min > 5 && s.Fg > 10);

	        var stats = await query
	            .Skip((page - 1) * pageSize)
	            .Take(pageSize)
	            .ToListAsync();

	        var totalPages = (int)Math.Ceiling(await query.CountAsync() / (double)pageSize);

	        var playerStatsDtoList = _mapper.Map<List<PlayerRegularStatsDto>>(stats);

	        var playerStatsResponse = new PlayerStatsResponse
	        {
	            Stats = playerStatsDtoList,
	            Pages = totalPages,
	            CurrentPage = page
	        };

	        response.Data = playerStatsResponse;
	        response.Success = true;
	    }
	    catch (Exception ex)
	    {
	        response.Success = false;
	        response.Message = $"Ocorreu um erro ao recuperar estatísticas dos jogadores: {ex.Message}";
	    }

	    return response;
	}


		public async Task<ServiceResponse<List<TeamRegularStatsDto>>> GetAllTeamRegularStats()
		{

			var season = _context.Seasons.OrderBy(s => s.Year).Last();

			var teamRegularStatsList = await _context.TeamRegularStats
				.Where(t => t.Season == season.Year)
				.Include(t => t.Team)
				.ToListAsync();

			var response = new ServiceResponse<List<TeamRegularStatsDto>>
			{
				Data = _mapper.Map<List<TeamRegularStatsDto>>(teamRegularStatsList)
			};

			return response;
		}

		public async Task<ServiceResponse<List<TeamPlayoffsStatsDto>>> GetAllTeamPlayoffsStats()
		{

			var season = _context.Seasons.OrderBy(s => s.Year).Last();

			var teamRegularStatsList = await _context.TeamPlayoffsStats
				.Where(t => t.Season == season.Year)
				.Include(t => t.Team)
				.ToListAsync();

			var response = new ServiceResponse<List<TeamPlayoffsStatsDto>>
			{
				Data = _mapper.Map<List<TeamPlayoffsStatsDto>>(teamRegularStatsList)
			};

			return response;
		}


		public async Task<ServiceResponse<List<TeamRegularStatsRankDto>>> GetAllTeamRegularStatsRank()
		{

			var season = _context.Seasons.OrderBy(s => s.Year).Last();

			var teamRegularStatsList = await _context.TeamRegularStats
				.Where(t => t.Season == season.Year)
				.Include(t => t.Team)
				.ToListAsync();

			var response = new ServiceResponse<List<TeamRegularStatsRankDto>>
			{
				Data = _mapper.Map<List<TeamRegularStatsRankDto>>(teamRegularStatsList)
			};

			return response;
		}



		public async Task<ServiceResponse<List<PlayerPlayoffsStatsDto>>> GetAllPlayerPlayoffsStats()
		{

			var season = _context.Seasons.OrderBy(s => s.Year).Last();

			var playerRegularStatsList = await _context.PlayerPlayoffsStats
				.Where(t => t.Season == season.Year && t.Min > 5)
				.ToListAsync();

			var response = new ServiceResponse<List<PlayerPlayoffsStatsDto>>
			{
				Data = _mapper.Map<List<PlayerPlayoffsStatsDto>>(playerRegularStatsList)
			};

			return response;
		}



	}
}
