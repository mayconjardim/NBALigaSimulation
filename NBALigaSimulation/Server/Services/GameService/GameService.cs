using AutoMapper;
using NBALigaSimulation.Shared.Models;

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

        public async Task<ServiceResponse<bool>> CreateGame(CreateGameDto request)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            Game game = _mapper.Map<Game>(request);
            _context.Add(game);
            await _context.SaveChangesAsync();

            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateGame(int gameId)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            Game game = await _context.Games
                .Include(p => p.HomeTeam)
                .Include(p => p.AwayTeam)
                .FirstOrDefaultAsync(p => p.Id == gameId);

            if (game == null)
            {
                response.Success = false;
                response.Message = "Jogo não encontrado.";
                return response;
            }

            game.GameSim();

            await _context.SaveChangesAsync();

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
            var game = await _context.Games.Include(p => p.HomeTeam).Include(p => p.AwayTeam).FirstOrDefaultAsync(p => p.Id == gameId);

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


    }
}
