using AutoMapper;
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
                response.Success = false;
                response.Message = "Não há datas não simuladas disponíveis.";
                return response;
            }

            List<Game> games = await _context.Games
                .Include(p => p.HomeTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
                .Include(p => p.AwayTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
                .Include(p => p.HomeTeam.TeamRegularStats)
                .Include(p => p.AwayTeam.TeamRegularStats)
                .Include(p => p.HomeTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.RegularStats)
                .Include(p => p.AwayTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.RegularStats)
                .Where(g => g.GameDate == firstUnsimulatedDate && !g.Happened && g.Type == 0)
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
                    UpdateStandings();
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = $"Erro ao salvar alterações para o jogo com ID {game.Id}: {ex.Message}";
                    return response;
                }

            }

            response.Success = true;
            response.Data = true;
            return response;

        }

        public async Task<ServiceResponse<bool>> SimGameByDatePlayoffs()
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
                response.Success = false;
                response.Message = "Não há datas não simuladas disponíveis.";
                return response;
            }

            List<Game> games = await _context.Games
                .Include(p => p.HomeTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
                .Include(p => p.AwayTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.Ratings)
                .Include(p => p.HomeTeam.TeamPlayoffsStats)
                .Include(p => p.AwayTeam.TeamPlayoffsStats)
                .Include(p => p.HomeTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.PlayoffsStats)
                .Include(p => p.AwayTeam.Players.OrderBy(p => p.RosterOrder)).ThenInclude(p => p.PlayoffsStats)
                .Where(g => g.GameDate == firstUnsimulatedDate && !g.Happened && g.Type == 1)
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
                    UpdateStandings();
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = $"Erro ao salvar alterações para o jogo com ID {game.Id}: {ex.Message}";
                    return response;
                }

            }

            response.Success = true;
            response.Data = true;
            return response;
        }


        public async void UpdateStandings()
        {

            List<Team> teams = await _context.Teams.Where(t => t.IsHuman == true).ToListAsync();
            var season = await _context.Seasons.OrderBy(s => s.Id).LastOrDefaultAsync();


            List<TeamRegularStats> eastTeams = teams.Where(t => t.Conference == "East")
                                      .Select(t => t.TeamRegularStats.Find(trs => trs.Season == season.Year))
                                      .OrderByDescending(trs => trs.WinPct)
                                      .ToList();

            List<TeamRegularStats> westTeams = teams.Where(t => t.Conference == "West")
                                       .Select(t => t.TeamRegularStats.Find(trs => trs.Season == season.Year))
                                       .OrderByDescending(trs => trs.WinPct)
                                       .ToList();

            foreach (var team in eastTeams)
            {
                int index = eastTeams.IndexOf(team);
                team.ConfRank = index + 1;
            }

            foreach (var team in westTeams)
            {
                int index = westTeams.IndexOf(team);
                team.ConfRank = index + 1;
            }

        }

    }
}
