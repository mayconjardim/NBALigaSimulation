	public async Task<ServiceResponse<List<TeamRegularStatsDto>>> GetAllTeamRegularStats(int season, bool isAscending, string stat = null)
		{
			 var response = new ServiceResponse<List<TeamRegularStatsDto>>();

		    try
		    {
			    IQueryable<TeamRegularStats> query = _context.TeamRegularStats.Include(p => p.Team);

			    var orderByExpression = query.OrderByDescending(p => ((p.HomeWins + p.RoadWins) / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) ));

		        if (!string.IsNullOrEmpty(stat))
		        {
		            orderByExpression = stat switch
		            {
		                "GP" => isAscending ? query.OrderByDescending(p => (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) : query.OrderBy(p => (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) ),
		                "WIN%" => isAscending ? query.OrderByDescending(p => (p.HomeWins + p.RoadWins) / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) ) : query.OrderBy(p => (p.HomeWins + p.RoadWins) / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) ),
		                "W" => isAscending ? query.OrderByDescending(p => ((double)p.HomeWins / p.RoadWins)) : query.OrderBy(p => ((double)p.HomeWins / p.RoadWins)),
		                "L" => isAscending ? query.OrderByDescending(p => ((double)p.RoadWins / p.RoadWins)) : query.OrderBy(p => ((double)p.RoadWins / p.RoadWins)),
		                "PTS" => isAscending ? query.OrderByDescending(p => ((double)p.Points / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) : query.OrderBy(p => ((double)p.Points / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )),
		                "FGM" => isAscending ? query.OrderByDescending(p => ((double)p.FGM / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) : query.OrderBy(p => ((double)p.FGM / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )),
		                "FGA" => isAscending ? query.OrderByDescending(p => ((double)p.FGA / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) : query.OrderBy(p => ((double)p.FGA / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) ,
		                "FG%" => isAscending ? query.OrderByDescending(p => ((double)p.FGM / p.FGA * 100)) : query.OrderBy(p => ((double)p.FGM / p.FGA * 100)),
		                "3PM" => isAscending ? query.OrderByDescending(p => ((double)p.TPM / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) : query.OrderBy(p => ((double)p.TPM / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )),
		                "3PA" => isAscending ? query.OrderByDescending(p => ((double)p.TPA / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) : query.OrderBy(p => ((double)p.TPA / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )),
		                "3P%" => isAscending ? query.OrderByDescending(p => ((double)p.TPM / p.TPA * 100)) : query.OrderBy(p => ((double)p.TPM / p.TPA * 100)),
		                "FTM" => isAscending ? query.OrderByDescending(p => ((double)p.FTM / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) : query.OrderBy(p => ((double)p.FTM / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) ,
		                "FTA" => isAscending ? query.OrderByDescending(p => ((double)p.FTA / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) ))  : query.OrderBy(p => ((double)p.FTA / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )),
		                "FT%" => isAscending ? query.OrderByDescending(p => ((double)p.FTM / p.FTA * 100)) : query.OrderBy(p => ((double)p.FTM / p.FTA * 100)),
		                "REB" => isAscending ? query.OrderByDescending(p => ((double)p.Rebounds / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) : query.OrderBy(p => ((double)p.Rebounds / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )),
		                "AREB" => isAscending ? query.OrderByDescending(p => ((double)p.AllowedRebounds / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) : query.OrderBy(p => ((double)p.AllowedRebounds / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )),
		                "APG" => isAscending ? query.OrderByDescending(p => ((double)p.Assists / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) : query.OrderBy(p => ((double)p.Assists / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )),
		                "AAPG" => isAscending ? query.OrderByDescending(p => ((double)p.AllowedAssists / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) : query.OrderBy(p => ((double)p.AllowedAssists / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )),
		                "SPG" => isAscending ? query.OrderByDescending(p => ((double)p.Steals / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) : query.OrderBy(p => ((double)p.Steals / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )),
		                "ASPG" => isAscending ? query.OrderByDescending(p => ((double)p.AllowedStealS / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) : query.OrderBy(p => ((double)p.AllowedStealS / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )),
			            "BPG" => isAscending ? query.OrderByDescending(p => ((double)p.Blocks / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) : query.OrderBy(p => ((double)p.Blocks / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )),
			            "ABPG" => isAscending ? query.OrderByDescending(p => ((double)p.AllowedBlocks / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) : query.OrderBy(p => ((double)p.AllowedBlocks / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )),
			            "TPG" => isAscending ? query.OrderByDescending(p => ((double)p.Turnovers / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) : query.OrderBy(p => ((double)p.Turnovers / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )),
			            "ATPG" => isAscending ? query.OrderByDescending(p => ((double)p.AllowedTurnovers / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) : query.OrderBy(p => ((double)p.AllowedTurnovers / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )),
		                "FPG" => isAscending ? query.OrderByDescending(p => ((double)p.Fouls / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) : query.OrderBy(p => ((double)p.Fouls / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )),
		                "AFPG" => isAscending ? query.OrderByDescending(p => ((double)p.AllowedFouls / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) : query.OrderBy(p => ((double)p.AllowedFouls / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )),
		                "DRAT" => isAscending ? query.OrderByDescending(p => ((double) (1.0 / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) ) * ((p.Points * 85.910) + (p.Rebounds * 53.840) + (p.Assists * 34.677) + (p.Steals * 53.840) +
			                (p.Blocks * 53.840) - (p.FGA * 39.190) - (p.FTA * 20.091) - (p.Turnovers * 53.840)) / 100)) : query.OrderBy(p => ((double) (1.0 / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) ) * ((p.Points * 85.910) + (p.Rebounds * 53.840) 
							+ (p.Assists * 34.677) + (p.Steals * 53.840) +
			                (p.Blocks * 53.840) - (p.FGA * 39.190) - (p.FTA * 20.091) - (p.Turnovers * 53.840)) / 100)),
		                
		                "OFAT" => isAscending ? query.OrderByDescending(p => ((double) (1.0 / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) * ((p.AllowedPoints * 85.910) + (p.AllowedRebounds * 53.840) + (p.AllowedAssists * 34.677) + (p.AllowedStealS * 53.840) + (p.AllowedBlocks * 53.840) -
			                (p.AllowedFGA * 39.190) - (p.AllowedFTA * 20.091) - (p.AllowedTurnovers * 53.840))/ 100) : query.OrderBy(p => ((double) (1.0 / (p.HomeWins + p.RoadWins + p.RoadLosses + p.RoadLosses)) )) * ((p.AllowedPoints * 85.910) + (p.AllowedRebounds * 53.840) + (p.AllowedAssists * 34.677) + 
							(p.AllowedStealS * 53.840) + (p.AllowedBlocks * 53.840) -
			                (p.AllowedFGA * 39.190) - (p.AllowedFTA * 20.091) - (p.AllowedTurnovers * 53.840))/ 100),
		                
		                _ => orderByExpression
		            };

		            query = orderByExpression.Where(p => p.Season == season);

		            var stats = await query.ToListAsync();

		            var teamStats = _mapper.Map<List<TeamRegularStatsDto>>(stats);

		            response.Data = teamStats;
		            response.Message = "Stats carregadas com sucesso!";
		            response.Success = true;
		        }
		  
		    }
		    catch (Exception ex)
		    {
		        response.Success = false;
		        response.Message = $"Ocorreu um erro ao recuperar estatísticas dos jogadores: {ex.Message}";
		    }

		    return response;
	
		}