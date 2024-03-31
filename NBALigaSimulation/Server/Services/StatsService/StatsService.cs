using AutoMapper;
using NBALigaSimulation.Shared.Models.Teams;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;
using PlayerRegularStats = NBALigaSimulation.Shared.Models.Players.PlayerRegularStats;

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
		
       public async Task<ServiceResponse<PageableStatsResponse<PlayerRegularStatsDto>>> GetAllPlayerRegularStats(int page, int pageSize, int season, bool isAscending, string position, string stat = null)
		{
		    var response = new ServiceResponse<PageableStatsResponse<PlayerRegularStatsDto>>();

		    try
		    {
			    IQueryable<PlayerRegularStats> query = _context.PlayerRegularStats.Include(p => p.Player);

                var orderByExpression = query.OrderByDescending(p => (p.Pts / p.Games));

		        if (!string.IsNullOrEmpty(stat))
		        {
		            orderByExpression = stat switch
		            {
		                "GP" => isAscending ? query.OrderByDescending(p => p.Games) : query.OrderBy(p => p.Games),
		                "MIN" => isAscending ? query.OrderByDescending(p => ((double)p.Min / p.Games)) : query.OrderBy(p => ((double)p.Min / p.Games)),
		                "FG%" => isAscending ? query.OrderByDescending(p => ((double)p.Fg / p.Fga * 100)) : query.OrderBy(p => ((double)p.Fg / p.Fga * 100)),
		                "3P%" => isAscending ? query.OrderByDescending(p => ((double)p.Tp / p.Tpa * 100)) : query.OrderBy(p => ((double)p.Tp / p.Tpa * 100)),
			            "FT%" => isAscending ? query.OrderByDescending(p => ((double)p.Ft / p.Fta * 100)) : query.OrderBy(p => ((double)p.Ft / p.Fta * 100)),
		                "ORB" => isAscending ? query.OrderByDescending(p => ((double)p.Orb / p.Games)) : query.OrderBy(p => ((double)p.Orb / p.Games)),
		                "DRB" => isAscending ? query.OrderByDescending(p => ((double)p.Drb / p.Games)) : query.OrderBy(p => ((double)p.Drb / p.Games)),
		                "RPG" => isAscending ? query.OrderByDescending(p => ((double)p.Trb / p.Games)) : query.OrderBy(p => ((double)p.Trb / p.Games)),
		                "APG" => isAscending ? query.OrderByDescending(p => ((double)p.Ast / p.Games)) : query.OrderBy(p => ((double)p.Ast / p.Games)),
		                "SPG" => isAscending ? query.OrderByDescending(p => ((double)p.Stl / p.Games)) : query.OrderBy(p => ((double)p.Stl / p.Games)),
		                "BPG" => isAscending ? query.OrderByDescending(p => ((double)p.Blk / p.Games)) : query.OrderBy(p => ((double)p.Blk / p.Games)),
		                "TPG" => isAscending ? query.OrderByDescending(p => ((double)p.Tov / p.Games)) : query.OrderBy(p => ((double)p.Tov / p.Games)),
			            "FPG" => isAscending ? query.OrderByDescending(p => ((double)p.Pf / p.Games)) : query.OrderBy(p => ((double)p.Pf / p.Games)),
		                "TS%" => isAscending ? query.OrderByDescending(p => (((double)p.Pts / (2.0 * (p.Fga + (0.44 * p.Fta)))) * 100)) : query.OrderBy(p => ((p.Pts / (2.0 * (p.Fga + (0.44 * p.Fta)))) * 100)),
		                _ => orderByExpression
		            };
		        }
		        
		        query = orderByExpression.Where(s => s.Season == season && s.Min > 5 && s.Fg > 10);
		        
		        if (!string.IsNullOrEmpty(position))
		        {
			        query = orderByExpression.Where(s => s.Season == season && s.Min > 5 && s.Fg > 10 && s.Pos == position);
		        }

		        var stats = await query
		            .Skip((page - 1) * pageSize)
		            .Take(pageSize)
		            .ToListAsync();

		        var totalPages = (int)Math.Ceiling(await query.CountAsync() / (double)pageSize);

		        var playerStatsDtoList = _mapper.Map<List<PlayerRegularStatsDto>>(stats);

		        var playerStatsResponse = new PageableStatsResponse<PlayerRegularStatsDto>()
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

		public async Task<ServiceResponse<List<TeamRegularStatsDto>>> GetAllTeamRegularStats(int season, bool isAscending, string stat = null)
		{
			 var response = new ServiceResponse<List<TeamRegularStatsDto>>();

		    try
		    {
			    IQueryable<TeamRegularStats> query = _context.TeamRegularStats.Include(p => p.Team);

                var orderByExpression = query.OrderByDescending(p => (p.WinPct));

		        if (!string.IsNullOrEmpty(stat))
		        {
		            orderByExpression = stat switch
		            {
		                "GP" => isAscending ? query.OrderByDescending(p => p.Games) : query.OrderBy(p => p.Games),
		                
		                "W" => isAscending ? query.OrderByDescending(p => ((double)p.HomeWins / p.RoadWins)) : query.OrderBy(p => ((double)p.HomeWins / p.RoadWins)),
		                "L" => isAscending ? query.OrderByDescending(p => ((double)p.RoadWins / p.RoadWins)) : query.OrderBy(p => ((double)p.RoadWins / p.RoadWins)),
		                
		                "PTS" => isAscending ? query.OrderByDescending(p => ((double)p.Points / p.Games)) : query.OrderBy(p => ((double)p.Points / p.Games)),
		               
		                "FGM" => isAscending ? query.OrderByDescending(p => ((double)p.FGM / p.Games)) : query.OrderBy(p => ((double)p.FGM / p.Games)),
		                "FGA" => isAscending ? query.OrderByDescending(p => ((double)p.FGA / p.Games)) : query.OrderBy(p => ((double)p.FGA / p.Games)) ,
		                "FG%" => isAscending ? query.OrderByDescending(p => ((double)p.FGM / p.FGA * 100)) : query.OrderBy(p => ((double)p.FGM / p.FGA * 100)),
		                
		                "3PM" => isAscending ? query.OrderByDescending(p => ((double)p.TPM / p.Games)) : query.OrderBy(p => ((double)p.TPM / p.Games)),
		                "3PA" => isAscending ? query.OrderByDescending(p => ((double)p.TPA / p.Games)) : query.OrderBy(p => ((double)p.TPA / p.Games)),
		                "3P%" => isAscending ? query.OrderByDescending(p => ((double)p.TPM / p.TPA * 100)) : query.OrderBy(p => ((double)p.TPM / p.TPA * 100)),
		                
		                "FTM" => isAscending ? query.OrderByDescending(p => ((double)p.FTM / p.Games)) : query.OrderBy(p => ((double)p.FTM / p.Games)) ,
		                "FTA" => isAscending ? query.OrderByDescending(p => ((double)p.FTA / p.Games))  : query.OrderBy(p => ((double)p.FTA / p.Games)),
		                "FT%" => isAscending ? query.OrderByDescending(p => ((double)p.FTM / p.FTA * 100)) : query.OrderBy(p => ((double)p.FTM / p.FTA * 100)),
  
		                
		                "REB" => isAscending ? query.OrderByDescending(p => ((double)p.Rebounds / p.Games)) : query.OrderBy(p => ((double)p.Rebounds / p.Games)),
		                "AREB" => isAscending ? query.OrderByDescending(p => ((double)p.AllowedRebounds / p.Games)) : query.OrderBy(p => ((double)p.AllowedRebounds / p.Games)),
		                
		                "APG" => isAscending ? query.OrderByDescending(p => ((double)p.Assists / p.Games)) : query.OrderBy(p => ((double)p.Assists / p.Games)),
		                "AAPG" => isAscending ? query.OrderByDescending(p => ((double)p.AllowedAssists / p.Games)) : query.OrderBy(p => ((double)p.AllowedAssists / p.Games)),

		                "SPG" => isAscending ? query.OrderByDescending(p => ((double)p.Steals / p.Games)) : query.OrderBy(p => ((double)p.Steals / p.Games)),
		                "ASPG" => isAscending ? query.OrderByDescending(p => ((double)p.AllowedStealS / p.Games)) : query.OrderBy(p => ((double)p.AllowedStealS / p.Games)),

			            "BPG" => isAscending ? query.OrderByDescending(p => ((double)p.Blocks / p.Games)) : query.OrderBy(p => ((double)p.Blocks / p.Games)),
			            "ABPG" => isAscending ? query.OrderByDescending(p => ((double)p.AllowedBlocks / p.Games)) : query.OrderBy(p => ((double)p.AllowedBlocks / p.Games)),
		                
			            "TPG" => isAscending ? query.OrderByDescending(p => ((double)p.Turnovers / p.Games)) : query.OrderBy(p => ((double)p.Turnovers / p.Games)),
			            "ATPG" => isAscending ? query.OrderByDescending(p => ((double)p.AllowedTurnovers / p.Games)) : query.OrderBy(p => ((double)p.AllowedTurnovers / p.Games)),

		                "FPG" => isAscending ? query.OrderByDescending(p => ((double)p.Fouls / p.Games)) : query.OrderBy(p => ((double)p.Fouls / p.Games)),
		                "AFPG" => isAscending ? query.OrderByDescending(p => ((double)p.AllowedFouls / p.Games)) : query.OrderBy(p => ((double)p.AllowedFouls / p.Games)),

		                "DRAT" => isAscending ? query.OrderByDescending(p => ((double) (1.0 / p.Games) * ((p.Points * 85.910) + (p.Rebounds * 53.840) + (p.Assists * 34.677) + (p.Steals * 53.840) +
			                (p.Blocks * 53.840) - (p.FGA * 39.190) - (p.FTA * 20.091) - (p.Turnovers * 53.840)) / 100)) : query.OrderBy(p => ((double) (1.0 / p.Games) * ((p.Points * 85.910) + (p.Rebounds * 53.840) 
							+ (p.Assists * 34.677) + (p.Steals * 53.840) +
			                (p.Blocks * 53.840) - (p.FGA * 39.190) - (p.FTA * 20.091) - (p.Turnovers * 53.840)) / 100)),
		                
		                "OFAT" => isAscending ? query.OrderByDescending(p => ((double) (1.0 / p.Games)) * ((p.AllowedPoints * 85.910) + (p.AllowedRebounds * 53.840) + (p.AllowedAssists * 34.677) + (p.AllowedStealS * 53.840) + (p.AllowedBlocks * 53.840) -
			                (p.AllowedFGA * 39.190) - (p.AllowedFTA * 20.091) - (p.AllowedTurnovers * 53.840))/ 100) : query.OrderBy(p => ((double) (1.0 / p.Games)) * ((p.AllowedPoints * 85.910) + (p.AllowedRebounds * 53.840) + (p.AllowedAssists * 34.677) + 
							(p.AllowedStealS * 53.840) + (p.AllowedBlocks * 53.840) -
			                (p.AllowedFGA * 39.190) - (p.AllowedFTA * 20.091) - (p.AllowedTurnovers * 53.840))/ 100),
		                
		                _ => orderByExpression
		            };
		        }
		        
		        var stats = await query.ToListAsync();

		        var teamStatsDtoList = _mapper.Map<List<TeamRegularStatsDto>>(stats);
		        
		        response.Data = teamStatsDtoList;
		        response.Success = true;
		    }
		    catch (Exception ex)
		    {
		        response.Success = false;
		        response.Message = $"Ocorreu um erro ao recuperar estatísticas dos jogadores: {ex.Message}";
		    }

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
