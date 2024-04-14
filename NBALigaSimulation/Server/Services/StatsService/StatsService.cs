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
				 
				 var orderByExpression = query.OrderByDescending(p => ( (double) (p.HomeWins + p.RoadWins) / (p.HomeWins + p.RoadWins + p.HomeLosses + p.RoadLosses)));

				 if (!string.IsNullOrEmpty(stat))
				 {
					  orderByExpression = stat switch
					 {

						 "GP" => isAscending ? query.OrderByDescending(p => (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)) : query.OrderBy(p => (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)),
						 "WIN%" => isAscending ? query.OrderByDescending(p => ( (double) (p.HomeWins + p.RoadWins) / (+ p.HomeWins + p.RoadWins +p.HomeLosses + p.RoadLosses))): query.OrderBy(p => ( (double) (p.HomeWins + p.RoadWins) / (+ p.HomeWins + p.RoadWins +p.HomeLosses + p.RoadLosses))),
						 "W" => isAscending
							 ? query.OrderByDescending(p => (p.HomeWins + p.RoadWins))
							 : query.OrderBy(p => (p.HomeWins + p.RoadWins)),
						 "L" => isAscending
							 ? query.OrderByDescending(p => (p.HomeLosses + p.RoadLosses))
							 : query.OrderBy(p => (p.HomeLosses + p.RoadLosses)),
						 "PTS" => isAscending
							 ? query.OrderByDescending(p => ((double)p.Points / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.Points / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 "FGM" => isAscending
							 ? query.OrderByDescending(p => ((double)p.FGM / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.FGM / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 "FGA" => isAscending
							 ? query.OrderByDescending(p => ((double)p.FGA / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.FGA / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 "FG%" => isAscending
							 ? query.OrderByDescending(p => ((double)p.FGM / p.FGA * 100))
							 : query.OrderBy(p => ((double)p.FGM / p.FGA * 100)),
						 
						 "3PM" => isAscending
							 ? query.OrderByDescending(p => ((double)p.TPM / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.TPM / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 "3PA" => isAscending
							 ? query.OrderByDescending(p => ((double)p.TPA / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.TPA / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 "3P%" => isAscending
							 ? query.OrderByDescending(p => ((double)p.TPM / p.TPA * 100))
							 : query.OrderBy(p => ((double)p.TPM / p.TPA * 100)),
						 "FTM" => isAscending
							 ? query.OrderByDescending(p => ((double)p.FTM / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.FTM / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 "FTA" => isAscending
							 ? query.OrderByDescending(p => ((double)p.FTA / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.FTA / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 "FT%" => isAscending
							 ? query.OrderByDescending(p => ((double)p.FTM / p.FTA * 100))
							 : query.OrderBy(p => ((double)p.FTM / p.FTA * 100)),
						 "REB" => isAscending
							 ? query.OrderByDescending(p => ((double)p.Rebounds / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.Rebounds / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 "AREB" => isAscending
							 ? query.OrderByDescending(p => ((double)p.AllowedRebounds / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.AllowedRebounds / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 "APG" => isAscending
							 ? query.OrderByDescending(p => ((double)p.Assists / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.Assists / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 "AAPG" => isAscending
							 ? query.OrderByDescending(p => ((double)p.AllowedAssists / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.AllowedAssists / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 "SPG" => isAscending
							 ? query.OrderByDescending(p => ((double)p.Steals / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.Steals / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 "ASPG" => isAscending
							 ? query.OrderByDescending(p => ((double)p.AllowedStealS / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.AllowedStealS / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 "BPG" => isAscending
							 ? query.OrderByDescending(p => ((double)p.Blocks / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.Blocks / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 "ABPG" => isAscending
							 ? query.OrderByDescending(p => ((double)p.AllowedBlocks / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.AllowedBlocks / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 "TPG" => isAscending
							 ? query.OrderByDescending(p => ((double)p.Turnovers / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.Turnovers / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 "ATPG" => isAscending
							 ? query.OrderByDescending(p => ((double)p.AllowedTurnovers / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.AllowedTurnovers / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 "FPG" => isAscending
							 ? query.OrderByDescending(p => ((double)p.Fouls / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.Fouls / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 "AFPG" => isAscending
							 ? query.OrderByDescending(p => ((double)p.AllowedFouls / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses)))
							 : query.OrderBy(p => ((double)p.AllowedFouls / (p.HomeWins + p.HomeLosses + p.RoadWins + p.RoadLosses))),
						 _ => orderByExpression
					 };
				 }
				 
				 query = orderByExpression.Where(s => s.Season == season);
					  
				 var stats = await query
					 .ToListAsync();

				 var teamStats = _mapper.Map<List<TeamRegularStatsDto>>(stats);

				 response.Data = teamStats;
				 response.Message = "Stats carregadas com sucesso!";
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
