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
		
       public async Task<ServiceResponse<PlayerStatsResponse>> GetAllPlayerRegularStats(int page, int pageSize, int season, bool isAscending, string stat = null)
		{
		    var response = new ServiceResponse<PlayerStatsResponse>();

		    try
		    {
		        IQueryable<PlayerRegularStats> query = _context.PlayerRegularStats.Include(p => p.Player);

		        var orderByExpression = query.OrderByDescending(p => (p.Pts / p.Games));

		        if (!string.IsNullOrEmpty(stat))
		        {
		            orderByExpression = stat switch
		            {
			            "PPG" => isAscending ? query.OrderByDescending(p => ((double)p.Pts / p.Games)) : query.OrderByDescending(p => ((double)p.Pts / p.Games)),
		                "GP" => isAscending ? query.OrderByDescending(p => p.Games) : query.OrderByDescending(p => p.Games),
		                "MIN" => isAscending ? query.OrderByDescending(p => ((double)p.Min / p.Games)) : query.OrderByDescending(p => ((double)p.Min / p.Games)),
		                "FG%" => isAscending ? query.OrderByDescending(p => ((double)p.Fg / p.Fga * 100)) : query.OrderByDescending(p => ((double)p.Fg / p.Fga * 100)),
		                "3P%" => isAscending ? query.OrderByDescending(p => ((double)p.Tp / p.Tpa * 100)) : query.OrderByDescending(p => ((double)p.Tp / p.Tpa * 100)),
			            "FT%" => isAscending ? query.OrderByDescending(p => ((double)p.Ft / p.Fta * 100)) : query.OrderByDescending(p => ((double)p.Ft / p.Fta * 100)),
		                "ORB" => isAscending ? query.OrderByDescending(p => ((double)p.Orb / p.Games)) : query.OrderByDescending(p => ((double)p.Orb / p.Games)),
		                "DRB" => isAscending ? query.OrderByDescending(p => ((double)p.Drb / p.Games)) : query.OrderByDescending(p => ((double)p.Drb / p.Games)),
		                "RPG" => isAscending ? query.OrderByDescending(p => ((double)p.Trb / p.Games)) : query.OrderByDescending(p => ((double)p.Trb / p.Games)),
		                "APG" => isAscending ? query.OrderByDescending(p => ((double)p.Ast / p.Games)) : query.OrderByDescending(p => ((double)p.Ast / p.Games)),
		                "SPG" => isAscending ? query.OrderByDescending(p => ((double)p.Stl / p.Games)) : query.OrderByDescending(p => ((double)p.Stl / p.Games)),
		                "BPG" => isAscending ? query.OrderByDescending(p => ((double)p.Blk / p.Games)) : query.OrderByDescending(p => ((double)p.Blk / p.Games)),
		                "TPG" => isAscending ? query.OrderByDescending(p => ((double)p.Tov / p.Games)) : query.OrderByDescending(p => ((double)p.Tov / p.Games)),
			            "FPG" => isAscending ? query.OrderByDescending(p => ((double)p.Pf / p.Games)) : query.OrderByDescending(p => ((double)p.Pf / p.Games)),
		                "TS%" => isAscending ? query.OrderByDescending(p => (((double)p.Pts / (2.0 * (p.Fga + (0.44 * p.Fta)))) * 100)) : query.OrderByDescending(p => ((p.Pts / (2.0 * (p.Fga + (0.44 * p.Fta)))) * 100)),
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
