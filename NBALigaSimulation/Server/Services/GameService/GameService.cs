using AutoMapper;

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

            game.GameSim(); //Simula o jogo <----------

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

            var games = await _context.Games
            .Include(p => p.HomeTeam)
            .Include(p => p.AwayTeam)
            .Where(g => g.HomeTeamId == teamId || g.AwayTeamId == teamId).ToListAsync();

            var response = new ServiceResponse<List<GameCompleteDto>>
            {
                Data = _mapper.Map<List<GameCompleteDto>>(games)
            };

            return response;
        }
    }
}
