﻿using AutoMapper;
using NBALigaSimulation.Server.Services.PlayoffsService;
using NBALigaSimulation.Shared.Dtos.Games;
using NBALigaSimulation.Shared.Engine.GameSim.GameStart;
using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.GameNews;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Utils;
using System.Diagnostics;

namespace NBALigaSimulation.Server.Services.GameService
{
    public class GameService : IGameService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ITeamService _teamService;
        private readonly IPlayoffsService _playoffsService;


        public GameService(DataContext context, IMapper mapper, ITeamService teamService, IPlayoffsService playoffsService)
        {
            _context = context;
            _mapper = mapper;
            _teamService = teamService;
            _playoffsService = playoffsService;
        }
        
        public async Task<ServiceResponse<List<GameCompleteDto>>> GetGamesByTeamId(int teamId)
        {
            var response = new ServiceResponse<List<GameCompleteDto>>();

            try
            {
                var games = await _context.Games
                    .Where(g => g.HomeTeamId == teamId || g.AwayTeamId == teamId)
                    .OrderBy(g => g.GameDate.Date)
                    .Include(p => p.HomeTeam)
                    .Include(p => p.AwayTeam)
                    .Include(p => p.TeamGameStats)
                    .ToListAsync();

                response.Data = _mapper.Map<List<GameCompleteDto>>(games);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Ocorreu um erro ao buscar os jogos para o Time com o ID {teamId}: {ex.Message}";
            }

            return response;
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
              .Include(t => t.HomeTeam.Gameplan)
              .Include(t => t.AwayTeam.Gameplan)
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

			var sw = new Stopwatch();
			sw.Start();

			await _context.SaveChangesAsync();

            GameSimulation.Sim(game);

            await _context.SaveChangesAsync();

			sw.Stop();
			Console.WriteLine("Tempo gasto : " + sw.ElapsedMilliseconds.ToString() + " milisegundos");

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
                  .Include(t => t.HomeTeam.Gameplan)
                  .Include(t => t.AwayTeam.Gameplan)
                  .FirstOrDefaultAsync(p => p.Id == gameId);

                if (game == null)
                {
                    response.Success = false;
                    response.Message = $"Jogo com ID {gameId} não encontrado.";
                    return response;
                }

                GameSimulation.Sim(game);

                await _context.SaveChangesAsync();
            }

            response.Success = true;
            response.Data = true;

            return response;
        }

        public async Task<ServiceResponse<List<GameCompleteDto>>> GetAllGames()
        {
            var games = await _context.Games.
                 Include(g => g.HomeTeam)
                .Include(g => g.AwayTeam)
                .Include(g => g.TeamGameStats)
                 .ToListAsync();
          
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

        public async Task<ServiceResponse<bool>> SimGameByDateRegular()
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            var season = await _context.Seasons.OrderBy(s => s.Id).LastOrDefaultAsync();

            DateTime? firstUnsimulatedDate = await _context.Games
             .Where(g => !g.Happened)
             .OrderBy(g => g.GameDate)
             .Select(g => g.GameDate)
             .FirstOrDefaultAsync();

            var unSimulatedDates = await _context.Games
                .Where(g => !g.Happened)
                .OrderBy(g => g.GameDate)
                .Select(g => g.GameDate)
                .ToListAsync();

            if (unSimulatedDates.Count == 0)
            {

                var teamRegularStats = await _context.TeamRegularStats.Where(t => t.Season == season.Year).ToListAsync();

                bool isRegularSeasonComplete = teamRegularStats.All(t => t.HomeLosses + t.HomeWins + t.RoadLosses + t.RoadWins == 74);

                if (isRegularSeasonComplete)
                {
                    var playoffs = await _playoffsService.GeneratePlayoffs();

                    response.Message = "Temporada regular finalizada, playoffs gerado!";
                    response.Success = true;
                    return response;
                }

                response.Message = "Não há datas não simuladas disponíveis.";
                response.Success = false;
                return response;
            }

            List<Game> games = await _context.Games
                .Include(p => p.HomeTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
                .Include(p => p.AwayTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
                .Include(p => p.HomeTeam.TeamRegularStats)
                .Include(p => p.AwayTeam.TeamRegularStats)
                .Include(p => p.HomeTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.RegularStats)
                .Include(p => p.AwayTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.RegularStats)
                .Where(g => g.GameDate == firstUnsimulatedDate && !g.Happened)
                .Include(t => t.HomeTeam.Gameplan)
                 .Include(t => t.AwayTeam.Gameplan)
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

                GameSimulation.Sim(game);
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
                    News news = SimulationUtils.NewGenerator(game);
                    await _context.AddAsync(news);

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
                  .Include(t => t.HomeTeam.Gameplan)
                  .Include(t => t.AwayTeam.Gameplan)
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
                        News news = SimulationUtils.NewGenerator(game);
                        await _context.AddAsync(news);
                        await _context.SaveChangesAsync();

                        if (playoffToUpdate.WinsTeamOne >= 4 || playoffToUpdate.WinsTeamTwo >= 4)
                        {
                            playoffToUpdate.Complete = true;
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

            if (eastTeams != null && westTeams != null)
            {
                for (int i = 0; i < eastTeams.Count; i++)
                {
                    if (eastTeams[i] != null)
                    {
                        eastTeams[i].ConfRank = i + 1;
                    }
                }

                for (int i = 0; i < westTeams.Count; i++)
                {
                    if (westTeams[i] != null)
                    {
                        westTeams[i].ConfRank = i + 1;
                    }
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
                .Include(t => t.HomeTeam.Gameplan)
                .Include(t => t.AwayTeam.Gameplan)
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

                GameSimulation.Sim(game);
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
                    News news = SimulationUtils.NewGenerator(game);
                    await _context.AddAsync(news);
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



