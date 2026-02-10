using AutoMapper;
using NBALigaSimulation.Server.Services.PlayoffsService;
using NBALigaSimulation.Shared.Dtos.Games;
using NBALigaSimulation.Shared.Engine.GameSim.GameStart;
using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.GameNews;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Teams;
using NBALigaSimulation.Shared.Models.SeasonPlayoffs;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Utils;
using System.Diagnostics;

namespace NBALigaSimulation.Server.Services.GameService
{
    public class GameService : IGameService
    {
        private readonly IGenericRepository<Game> _gameRepository;
        private readonly IGenericRepository<Team> _teamRepository;
        private readonly IGenericRepository<TeamRegularStats> _teamRegularStatsRepository;
        private readonly IGenericRepository<Playoffs> _playoffsRepository;
        private readonly IGenericRepository<PlayoffsGame> _playoffsGameRepository;
        private readonly IGenericRepository<News> _newsRepository;
        private readonly ISeasonRepository _seasonRepository;
        private readonly IMapper _mapper;
        private readonly ITeamService _teamService;
        private readonly IPlayoffsService _playoffsService;

        public GameService(
            IGenericRepository<Game> gameRepository,
            IGenericRepository<Team> teamRepository,
            IGenericRepository<TeamRegularStats> teamRegularStatsRepository,
            IGenericRepository<Playoffs> playoffsRepository,
            IGenericRepository<PlayoffsGame> playoffsGameRepository,
            IGenericRepository<News> newsRepository,
            ISeasonRepository seasonRepository,
            IMapper mapper,
            ITeamService teamService,
            IPlayoffsService playoffsService)
        {
            _gameRepository = gameRepository;
            _teamRepository = teamRepository;
            _teamRegularStatsRepository = teamRegularStatsRepository;
            _playoffsRepository = playoffsRepository;
            _playoffsGameRepository = playoffsGameRepository;
            _newsRepository = newsRepository;
            _seasonRepository = seasonRepository;
            _mapper = mapper;
            _teamService = teamService;
            _playoffsService = playoffsService;
        }
        
        public async Task<ServiceResponse<List<GameCompleteDto>>> GetGamesByTeamId(int teamId)
        {
            var response = new ServiceResponse<List<GameCompleteDto>>();

            try
            {
                var games = await _gameRepository.Query()
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
            Season season = await _seasonRepository.Query()
                .OrderBy(s => s.Id)
                .LastOrDefaultAsync();
            game.Season = season;

            await _gameRepository.AddAsync(game);
            await _gameRepository.SaveChangesAsync();

            response.Success = true;
            response.Data = _mapper.Map<GameCompleteDto>(game);
            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateGame(int gameId)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            Game game = await _gameRepository.Query()
              .Include(p => p.HomeTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
              .Include(p => p.AwayTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
              .Include(t => t.HomeTeam.Gameplan)
              .Include(t => t.AwayTeam.Gameplan)
              .FirstOrDefaultAsync(p => p.Id == gameId);

            game.Season = await _seasonRepository.Query()
                .OrderBy(s => s.Id)
                .LastOrDefaultAsync();

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

			await _gameRepository.SaveChangesAsync();

            GameSimulation.Sim(game);

            await _gameRepository.SaveChangesAsync();

			sw.Stop();
			Console.WriteLine("Tempo gasto : " + sw.ElapsedMilliseconds.ToString() + " milisegundos");

			response.Success = true;
            response.Data = true;

            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateGames()
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            List<int> gameIds = await _seasonRepository.Query()
                .OrderByDescending(s => s.Id)
                .SelectMany(s => s.Games)
                .Select(g => g.Id)
                .ToListAsync();

            foreach (int gameId in gameIds)
            {
                Game game = await _gameRepository.Query()
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

                await _gameRepository.SaveChangesAsync();
            }

            response.Success = true;
            response.Data = true;

            return response;
        }

        public async Task<ServiceResponse<List<GameCompleteDto>>> GetAllGames()
        {
            var games = await _gameRepository.Query()
                .Include(g => g.HomeTeam)
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
            var game = await _gameRepository.Query()
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

            var season = await _seasonRepository.Query()
                .OrderBy(s => s.Id)
                .LastOrDefaultAsync();

            DateTime? firstUnsimulatedDate = await _gameRepository.Query()
             .Where(g => !g.Happened)
             .OrderBy(g => g.GameDate)
             .Select(g => g.GameDate)
             .FirstOrDefaultAsync();

            var unSimulatedDates = await _gameRepository.Query()
                .Where(g => !g.Happened)
                .OrderBy(g => g.GameDate)
                .Select(g => g.GameDate)
                .ToListAsync();

            if (unSimulatedDates.Count == 0)
            {

                var teamRegularStats = await _teamRegularStatsRepository.Query()
                    .Where(t => t.Season == season.Year)
                    .ToListAsync();

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

                List<Game> games = await _gameRepository.Query()
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

                await _gameRepository.SaveChangesAsync();

                GameSimulation.Sim(game);
                game.Happened = true;


                try
                {
                    await _gameRepository.SaveChangesAsync();

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
                    await _newsRepository.AddAsync(news);

                    await _newsRepository.SaveChangesAsync();
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

            var season = await _seasonRepository.Query()
                .OrderBy(s => s.Id)
                .LastOrDefaultAsync();

            var playoffs = await _playoffsRepository.Query()
                .Where(p => p.Season == season.Year)
                .Include(p => p.PlayoffGames)
                .ToListAsync();

            DateTime gameDate = DateTime.MinValue;

            foreach (var playoff in playoffs)
            {
                var gameIds = playoff.PlayoffGames.Select(g => g.GameId).ToList();

                List<Game> games = await _gameRepository.Query()
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

                    await _gameRepository.SaveChangesAsync();

                    game.Happened = true;

                    await _gameRepository.SaveChangesAsync();

                    try
                    {
                        var playoffToUpdate = await _playoffsRepository.Query()
                            .FirstOrDefaultAsync(p => p.Id == playoff.Id);

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

                            _playoffsRepository.Update(playoffToUpdate);
                        }

                        SimulationUtils.UpdateTeamStats(game);
                        SimulationUtils.UpdatePlayerGames(game);
                        News news = SimulationUtils.NewGenerator(game);
                        await _newsRepository.AddAsync(news);
                        await _newsRepository.SaveChangesAsync();

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

            List<Game> remaining = await _gameRepository.Query()
                .Where(t => t.GameDate == gameDate && !t.Happened)
                .ToListAsync();

            foreach (var game in remaining)
            {
                _gameRepository.Remove(game);
            }

            await _gameRepository.SaveChangesAsync();

            response.Success = true;
            response.Data = true;
            return response;
        }

        public async Task UpdateStandings()
        {

            var season = await _seasonRepository.Query()
                .OrderBy(s => s.Year)
                .LastOrDefaultAsync();
            if (season == null)
            {
                return;
            }

            var teams = await _teamRepository.Query()
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

            await _teamRegularStatsRepository.SaveChangesAsync();

        }

        public async Task<ServiceResponse<bool>> SimAll()
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            var season = await _seasonRepository.Query()
                .OrderBy(s => s.Id)
                .LastOrDefaultAsync();

            List<Game> games = await _gameRepository.Query()
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

                await _gameRepository.SaveChangesAsync();

                GameSimulation.Sim(game);
                game.Happened = true;


                try
                {
                    UpdateStandings();
                    await _teamRegularStatsRepository.SaveChangesAsync();
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
                    await _newsRepository.AddAsync(news);
                    await _newsRepository.SaveChangesAsync();
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

        public async Task<ServiceResponse<bool>> SimGameByRound(int roundNumber)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            var season = await _seasonRepository.Query()
                .OrderBy(s => s.Id)
                .LastOrDefaultAsync();

            if (season == null)
            {
                response.Success = false;
                response.Message = "Temporada não encontrada.";
                return response;
            }

            // Busca todos os jogos da rodada especificada que ainda não foram simulados
            List<Game> games = await _gameRepository.Query()
                .Include(p => p.HomeTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
                .Include(p => p.AwayTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
                .Include(p => p.HomeTeam.TeamRegularStats)
                .Include(p => p.AwayTeam.TeamRegularStats)
                .Include(p => p.HomeTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.RegularStats)
                .Include(p => p.AwayTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.RegularStats)
                .Include(t => t.HomeTeam.Gameplan)
                .Include(t => t.AwayTeam.Gameplan)
                .Where(g => g.Week == roundNumber.ToString() && !g.Happened && g.SeasonId == season.Id)
                .ToListAsync();

            if (games.Count == 0)
            {
                response.Success = false;
                response.Message = $"Não há jogos não simulados na rodada {roundNumber}.";
                return response;
            }

            foreach (Game game in games)
            {
                game.Season = season;

                if (!SimulationUtils.ArePlayersInCorrectOrder(game.HomeTeam.Players))
                {
                    SimulationUtils.AdjustRosterOrder(game.HomeTeam.Players);
                }

                if (!SimulationUtils.ArePlayersInCorrectOrder(game.AwayTeam.Players))
                {
                    SimulationUtils.AdjustRosterOrder(game.AwayTeam.Players);
                }

                await _gameRepository.SaveChangesAsync();

                GameSimulation.Sim(game);
                game.Happened = true;

                try
                {
                    await _gameRepository.SaveChangesAsync();
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
                    await _newsRepository.AddAsync(news);
                    await _newsRepository.SaveChangesAsync();
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
            response.Message = $"Rodada {roundNumber} simulada com sucesso! {games.Count} jogos processados.";
            return response;
        }

        public async Task<ServiceResponse<bool>> SimPlayoffsByRound(int playoffRound)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            var season = await _seasonRepository.Query()
                .OrderBy(s => s.Id)
                .LastOrDefaultAsync();

            if (season == null)
            {
                response.Success = false;
                response.Message = "Temporada não encontrada.";
                return response;
            }

            Console.WriteLine($"[SimPlayoffsByRound] Iniciando simulação. Temporada={season.Year}, Round={playoffRound}");

            // Define os SeriesId para cada rodada de playoffs
            List<int> seriesIds = playoffRound switch
            {
                1 => new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 }, // Primeira rodada
                2 => new List<int> { 9, 10, 11, 12 }, // Semi-finais de conferência
                3 => new List<int> { 13, 14 }, // Finais de conferência
                4 => new List<int> { 15 }, // Final da NBA
                _ => new List<int>()
            };

            if (seriesIds.Count == 0)
            {
                response.Success = false;
                response.Message = $"Rodada de playoffs inválida: {playoffRound}";
                return response;
            }

            var playoffs = await _playoffsRepository.Query()
                .Where(p => p.Season == season.Year && seriesIds.Contains(p.SeriesId))
                .Include(p => p.PlayoffGames)
                .ToListAsync();

            Console.WriteLine($"[SimPlayoffsByRound] Séries encontradas para o round {playoffRound}: {playoffs.Count}");

            if (playoffs.Count == 0)
            {
                response.Success = false;
                response.Message = $"Não há séries de playoffs na rodada {playoffRound} para a temporada {season.Year}.";
                Console.WriteLine($"[SimPlayoffsByRound] {response.Message}");
                return response;
            }

            DateTime gameDate = DateTime.MinValue;
            int totalGamesProcessed = 0;

            // Para cada série de playoffs da rodada, simula seus jogos e atualiza o placar da série
            foreach (var playoff in playoffs)
            {
                var gameIds = playoff.PlayoffGames.Select(g => g.GameId).ToList();
                Console.WriteLine($"[SimPlayoffsByRound] Série Id={playoff.Id}, SeriesId={playoff.SeriesId}, Jogos vinculados={gameIds.Count}");

                List<Game> games = await _gameRepository.Query()
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

                if (games.Count == 0)
                {
                    Console.WriteLine($"[SimPlayoffsByRound] Nenhum jogo não simulado encontrado para a série Id={playoff.Id}.");
                    continue;
                }

                Console.WriteLine($"[SimPlayoffsByRound] Série Id={playoff.Id}: até {games.Count} jogos (para ao primeiro a 4 vitórias).");

                foreach (var game in games)
                {
                    totalGamesProcessed++;
                    game.Season = season;
                    gameDate = game.GameDate;

                    if (!SimulationUtils.ArePlayersInCorrectOrder(game.HomeTeam.Players))
                    {
                        SimulationUtils.AdjustRosterOrder(game.HomeTeam.Players);
                    }

                    if (!SimulationUtils.ArePlayersInCorrectOrder(game.AwayTeam.Players))
                    {
                        SimulationUtils.AdjustRosterOrder(game.AwayTeam.Players);
                    }

                    await _gameRepository.SaveChangesAsync();

                    GameSimulation.Sim(game);
                    game.Happened = true;

                    // A simulação preenche team.Stats; sincroniza para game.TeamGameStats para UpdateTeamStats/UpdatePlayerGames
                    foreach (var s in game.HomeTeam.Stats.Where(s => s.GameId == game.Id))
                        game.TeamGameStats.Add(s);
                    foreach (var s in game.AwayTeam.Stats.Where(s => s.GameId == game.Id))
                        game.TeamGameStats.Add(s);

                    try
                    {
                        await _gameRepository.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        response.Success = false;
                        response.Message = $"Erro ao salvar alterações para o jogo com ID {game.Id}: {ex.Message}";
                        Console.WriteLine($"[SimPlayoffsByRound] ERRO: {response.Message}");
                        return response;
                    }

                    try
                    {
                        // Atualiza o placar da série de playoffs com base no resultado do jogo
                        var playoffToUpdate = await _playoffsRepository.Query()
                            .FirstOrDefaultAsync(p => p.Id == playoff.Id);

                        if (playoffToUpdate != null)
                        {
                            // A simulação grava pontos em team.Stats, não em game.TeamGameStats
                            var teamOneStats = (playoff.TeamOneId == game.HomeTeamId ? game.HomeTeam : game.AwayTeam)?.Stats?.Find(s => s.GameId == game.Id);
                            var teamTwoStats = (playoff.TeamTwoId == game.HomeTeamId ? game.HomeTeam : game.AwayTeam)?.Stats?.Find(s => s.GameId == game.Id);
                            int teamOnePoints = teamOneStats?.Pts ?? 0;
                            int teamTwoPoints = teamTwoStats?.Pts ?? 0;

                            if (teamOnePoints > teamTwoPoints)
                            {
                                playoffToUpdate.WinsTeamOne += 1;
                            }
                            else if (teamTwoPoints > teamOnePoints)
                            {
                                playoffToUpdate.WinsTeamTwo += 1;
                            }

                            if (playoffToUpdate.WinsTeamOne >= 4 || playoffToUpdate.WinsTeamTwo >= 4)
                            {
                                playoffToUpdate.Complete = true;
                                _playoffsRepository.Update(playoffToUpdate);
                                await _playoffsRepository.SaveChangesAsync();
                                SimulationUtils.UpdateTeamStats(game);
                                SimulationUtils.UpdatePlayerGames(game);
                                var gameNews = SimulationUtils.NewGenerator(game);
                                await _newsRepository.AddAsync(gameNews);
                                await _newsRepository.SaveChangesAsync();
                                // Remove jogos não disputados da série (quem faz 4 primeiro ganha)
                                var restantes = await _playoffsGameRepository.Query()
                                    .Where(pg => pg.PlayoffsId == playoff.Id)
                                    .Include(pg => pg.Game)
                                    .ToListAsync();
                                foreach (var pg in restantes.Where(pg => !pg.Game.Happened))
                                {
                                    _playoffsGameRepository.Remove(pg);
                                    _gameRepository.Remove(pg.Game);
                                }
                                await _gameRepository.SaveChangesAsync();
                                break;
                            }
                            _playoffsRepository.Update(playoffToUpdate);
                        }

                        SimulationUtils.UpdateTeamStats(game);
                        SimulationUtils.UpdatePlayerGames(game);
                        News news = SimulationUtils.NewGenerator(game);
                        await _newsRepository.AddAsync(news);
                        await _newsRepository.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        response.Success = false;
                        response.Message = $"Erro ao salvar alterações para o jogo com ID {game.Id}: {ex.Message}";
                        Console.WriteLine($"[SimPlayoffsByRound] ERRO: {response.Message}");
                        return response;
                    }
                }
            }

            if (totalGamesProcessed == 0)
            {
                response.Success = false;
                response.Message = $"Não há jogos não simulados na rodada {playoffRound} dos playoffs.";
                Console.WriteLine($"[SimPlayoffsByRound] {response.Message}");
                return response;
            }

            // Remove jogos restantes na mesma data que não foram simulados
            List<Game> remaining = await _gameRepository.Query()
                .Where(t => t.GameDate == gameDate && !t.Happened)
                .ToListAsync();

            foreach (var game in remaining)
            {
                _gameRepository.Remove(game);
            }

            await _gameRepository.SaveChangesAsync();

            await UpdateStandings();
            response.Success = true;
            response.Data = true;
            response.Message = $"Rodada {playoffRound} dos playoffs simulada com sucesso! {totalGamesProcessed} jogos processados.";
            Console.WriteLine($"[SimPlayoffsByRound] SUCESSO: {response.Message}");
            return response;
        }

    }
}



