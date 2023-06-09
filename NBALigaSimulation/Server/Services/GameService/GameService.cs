﻿using AutoMapper;
using NBALigaSimulation.Shared.Engine.Utils;

namespace NBALigaSimulation.Server.Services.GameService
{
	public class GameService : IGameService
	{

		private readonly DataContext _context;
		private readonly IMapper _mapper;
		private readonly ITeamService _teamService;

		public GameService(DataContext context, IMapper mapper, ITeamService teamService)
		{
			_context = context;
			_mapper = mapper;
			_teamService = teamService;
		}

		public async Task<ServiceResponse<GameCompleteDto>> CreateGame(CreateGameDto request)
		{
			ServiceResponse<GameCompleteDto> response = new ServiceResponse<GameCompleteDto>();

			Game game = _mapper.Map<Game>(request);
			Season season = _context.Seasons.OrderBy(s => s.Id).LastOrDefault();
			game.Season = season;

			_context.Add(game);
			await _context.SaveChangesAsync();

			response.Success = true;
			response.Data = _mapper.Map<GameCompleteDto>(game);
			return response;
		}

		public async Task<ServiceResponse<bool>> UpdateGame(int gameId)
		{
			ServiceResponse<bool> response = new ServiceResponse<bool>();

			Game game = await _context.Games
			  .Include(p => p.HomeTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
			  .Include(p => p.AwayTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
			  .FirstOrDefaultAsync(p => p.Id == gameId);

			game.Season = await _context.Seasons.OrderBy(s => s.Id).LastOrDefaultAsync();


			if (game == null)
			{
				response.Success = false;
				response.Message = "Jogo não encontrado.";
				return response;
			}

			if (!SimulationUtils.ArePlayersInCorrectOrder(game.HomeTeam.Players))
			{
				SimulationUtils.AdjustRosterOrder(game.HomeTeam.Players);
			}

			if (!SimulationUtils.ArePlayersInCorrectOrder(game.AwayTeam.Players))
			{
				SimulationUtils.AdjustRosterOrder(game.AwayTeam.Players);
			}

			await _context.SaveChangesAsync();

			game.GameSim();

			await _context.SaveChangesAsync();

			response.Success = true;
			response.Data = true;

			return response;
		}

		public async Task<ServiceResponse<bool>> UpdateGames()
		{
			ServiceResponse<bool> response = new ServiceResponse<bool>();

			List<int> gameIds = await _context.Seasons.OrderByDescending(s => s.Id).SelectMany(s => s.Games)
				.Select(g => g.Id).ToListAsync();

			foreach (int gameId in gameIds)
			{
				Game game = await _context.Games
				  .Include(p => p.HomeTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
				  .Include(p => p.AwayTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
				  .FirstOrDefaultAsync(p => p.Id == gameId);

				if (game == null)
				{
					response.Success = false;
					response.Message = $"Jogo com ID {gameId} não encontrado.";
					return response;
				}

				game.GameSim(); // Simula o jogo

				await _context.SaveChangesAsync();
			}

			response.Success = true;
			response.Data = true;

			return response;
		}

		public async Task<ServiceResponse<List<GameCompleteDto>>> GetAllGames()
		{
			var games = await _context.Games.ToListAsync();
			var response = new ServiceResponse<List<GameCompleteDto>>
			{
				Data = _mapper.Map<List<GameCompleteDto>>(games)
			};

			return response;
		}

		public async Task<ServiceResponse<GameCompleteDto>> GetGameById(int gameId)
		{
			var response = new ServiceResponse<GameCompleteDto>();
			var game = await _context.Games
			.Include(p => p.HomeTeam)
			.Include(p => p.AwayTeam)
			.Include(p => p.TeamGameStats)
			.Include(p => p.PlayerGameStats)
			.FirstOrDefaultAsync(p => p.Id == gameId);

			if (game == null)
			{
				response.Success = false;
				response.Message = $"O Game com o Id {gameId} não existe!";
			}
			else
			{
				response.Data = _mapper.Map<GameCompleteDto>(game);
			}

			return response;
		}

		public async Task<ServiceResponse<List<GameCompleteDto>>> GetGamesByTeamId(int teamId)
		{

			var games = await _context.Games.OrderBy(g => g.GameDate)
			.Include(p => p.HomeTeam)
			.Include(p => p.AwayTeam)
			.Include(p => p.TeamGameStats)
			.Where(g => g.HomeTeamId == teamId || g.AwayTeamId == teamId).ToListAsync();

			var response = new ServiceResponse<List<GameCompleteDto>>
			{
				Data = _mapper.Map<List<GameCompleteDto>>(games)
			};

			return response;
		}

		public async Task<ServiceResponse<bool>> SimGameByDateRegular()
		{
			ServiceResponse<bool> response = new ServiceResponse<bool>();

			var season = await _context.Seasons.OrderBy(s => s.Id).LastOrDefaultAsync();

			DateTime? firstUnsimulatedDate = await _context.Games
				.Where(g => !g.Happened)
				.OrderBy(g => g.GameDate)
				.Select(g => g.GameDate)
				.FirstOrDefaultAsync();

			if (!firstUnsimulatedDate.HasValue)
			{
				response.Message = "Não há datas não simuladas disponíveis.";
			}

			List<Game> games = await _context.Games
				.Include(p => p.HomeTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
				.Include(p => p.AwayTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
				.Include(p => p.HomeTeam.TeamRegularStats)
				.Include(p => p.AwayTeam.TeamRegularStats)
				.Include(p => p.HomeTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.RegularStats)
				.Include(p => p.AwayTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.RegularStats)
				.Where(g => g.GameDate == firstUnsimulatedDate && !g.Happened)
				.ToListAsync();

			foreach (Game game in games)
			{
				game.Season = season;

				if (SimulationUtils.ArePlayersInCorrectOrder(game.HomeTeam.Players))
				{
					SimulationUtils.AdjustRosterOrder(game.HomeTeam.Players);
				}

				if (SimulationUtils.ArePlayersInCorrectOrder(game.AwayTeam.Players))
				{
					SimulationUtils.AdjustRosterOrder(game.AwayTeam.Players);
				}

				await _context.SaveChangesAsync();

				game.GameSim();
				game.Happened = true;


				try
				{
					await _context.SaveChangesAsync();
				}
				catch (Exception ex)
				{
					response.Success = false;
					response.Message = $"Erro ao salvar alterações para o jogo com ID {game.Id}: {ex.Message}";
					return response;
				}

				try
				{
					SimulationUtils.UpdateTeamStats(game);
					SimulationUtils.UpdatePlayerGames(game);
					await _context.SaveChangesAsync();
				}
				catch (Exception ex)
				{
					response.Success = false;
					response.Message = $"Erro ao salvar alterações para o jogo com ID {game.Id}: {ex.Message}";
					return response;
				}

			}




			await UpdateStandings();
			response.Success = true;
			response.Data = true;
			return response;

		}


		public async Task<ServiceResponse<bool>> SimGameByDatePlayoffs()
		{
			ServiceResponse<bool> response = new ServiceResponse<bool>();

			var season = await _context.Seasons.OrderBy(s => s.Id).LastOrDefaultAsync();

			var playoffs = await _context.Playoffs
			  .Where(p => p.Season == season.Year)
			  .Include(p => p.PlayoffGames)
			  .ToListAsync();

			DateTime gameDate = DateTime.MinValue;

			foreach (var playoff in playoffs)
			{
				var gameIds = playoff.PlayoffGames.Select(g => g.GameId).ToList();

				List<Game> games = await _context.Games
				  .Include(p => p.HomeTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
				  .Include(p => p.AwayTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
				  .Include(p => p.HomeTeam.TeamPlayoffsStats)
				  .Include(p => p.AwayTeam.TeamPlayoffsStats)
				  .Include(p => p.HomeTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.PlayoffsStats)
				  .Include(p => p.AwayTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.PlayoffsStats)
				  .Where(g => gameIds.Contains(g.Id) && !g.Happened && g.Type == 1)
				  .ToListAsync();

				foreach (var game in games)
				{
					game.Season = season;
					gameDate = game.GameDate;

					if (SimulationUtils.ArePlayersInCorrectOrder(game.HomeTeam.Players))
					{
						SimulationUtils.AdjustRosterOrder(game.HomeTeam.Players);
					}

					if (SimulationUtils.ArePlayersInCorrectOrder(game.AwayTeam.Players))
					{
						SimulationUtils.AdjustRosterOrder(game.AwayTeam.Players);
					}

					await _context.SaveChangesAsync();

					game.GameSim();
					game.Happened = true;

					await _context.SaveChangesAsync();

					try
					{
						var playoffToUpdate = await _context.Playoffs.FirstOrDefaultAsync(p => p.Id == playoff.Id);

						if (playoffToUpdate != null)
						{
							int teamOnePoints = game.TeamGameStats
							  .Where(stat => stat.TeamId == playoff.TeamOneId)
							  .Sum(stat => stat.Pts);

							int teamTwoPoints = game.TeamGameStats
							  .Where(stat => stat.TeamId == playoff.TeamTwoId)
							  .Sum(stat => stat.Pts);

							if (teamOnePoints > teamTwoPoints)
							{
								playoffToUpdate.WinsTeamOne += 1;

							}
							else if (teamTwoPoints > teamOnePoints)
							{
								playoffToUpdate.WinsTeamTwo += 1;
							}

							_context.Update(playoffToUpdate);
						}

						SimulationUtils.UpdateTeamStats(game);
						SimulationUtils.UpdatePlayerGames(game);
						await _context.SaveChangesAsync();

						if (playoffToUpdate.WinsTeamOne >= 4 || playoffToUpdate.WinsTeamTwo >= 4)
						{
							break;
						}
					}
					catch (Exception ex)
					{
						response.Success = false;
						response.Message = $"Erro ao salvar alterações para o jogo com ID {game.Id}: {ex.Message}";
						return response;
					}
				}
			}

			List<Game> remaining = await _context.Games
			.Where(t => t.GameDate == gameDate && !t.Happened)
			.ToListAsync();
			_context.Games.RemoveRange(remaining);
			await _context.SaveChangesAsync();

			response.Success = true;
			response.Data = true;
			return response;
		}


		public async Task UpdateStandings()
		{

			var season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();
			if (season == null)
			{
				return;
			}

			var teams = await _context.Teams
				.Include(t => t.TeamRegularStats)
				.Where(t => t.IsHuman)
				.ToListAsync();

			var eastTeams = teams
				.Where(t => t.Conference == "East")
				.Select(t => t.TeamRegularStats.FirstOrDefault(trs => trs.Season == season.Year))
				.OrderByDescending(trs => trs?.WinPct ?? 0)
				.ToList();

			var westTeams = teams
				.Where(t => t.Conference == "West")
				.Select(t => t.TeamRegularStats.FirstOrDefault(trs => trs.Season == season.Year))
				.OrderByDescending(trs => trs?.WinPct ?? 0)
				.ToList();

			if (westTeams != null && eastTeams != null)
			{
				for (int i = 0; i < eastTeams.Count; i++)
				{
					eastTeams[i].ConfRank = i + 1;
				}

				for (int i = 0; i < westTeams.Count; i++)
				{
					westTeams[i].ConfRank = i + 1;
				}
			}

			await _context.SaveChangesAsync();

		}

		public async Task<ServiceResponse<bool>> SimAll()
		{
			ServiceResponse<bool> response = new ServiceResponse<bool>();

			var season = await _context.Seasons.OrderBy(s => s.Id).LastOrDefaultAsync();

			List<Game> games = await _context.Games
				.Include(p => p.HomeTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
				.Include(p => p.AwayTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
				.Include(p => p.HomeTeam.TeamRegularStats)
				.Include(p => p.AwayTeam.TeamRegularStats)
				.Include(p => p.HomeTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.RegularStats)
				.Include(p => p.AwayTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.RegularStats)
				.Where(g => g.Happened == false)
				.ToListAsync();

			foreach (Game game in games)
			{
				game.Season = season;

				if (SimulationUtils.ArePlayersInCorrectOrder(game.HomeTeam.Players))
				{
					SimulationUtils.AdjustRosterOrder(game.HomeTeam.Players);
				}

				if (SimulationUtils.ArePlayersInCorrectOrder(game.AwayTeam.Players))
				{
					SimulationUtils.AdjustRosterOrder(game.AwayTeam.Players);
				}

				await _context.SaveChangesAsync();

				game.GameSim();
				game.Happened = true;


				try
				{
					UpdateStandings();
					await _context.SaveChangesAsync();
				}
				catch (Exception ex)
				{
					response.Success = false;
					response.Message = $"Erro ao salvar alterações para o jogo com ID {game.Id}: {ex.Message}";
					return response;
				}

				try
				{
					SimulationUtils.UpdateTeamStats(game);
					SimulationUtils.UpdatePlayerGames(game);
					await _context.SaveChangesAsync();
				}
				catch (Exception ex)
				{
					response.Success = false;
					response.Message = $"Erro ao salvar alterações para o jogo com ID {game.Id}: {ex.Message}";
					return response;
				}

			}

			await UpdateStandings();
			response.Success = true;
			response.Data = true;
			return response;

		}


	}
}
