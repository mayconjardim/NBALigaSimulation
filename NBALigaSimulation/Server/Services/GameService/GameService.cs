using AutoMapper;

namespace NBALigaSimulation.Server.Services.GameService
{
    public class GameService : IGameService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public GameService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<GameCompleteDto>> CreateGame(GameCompleteDto request)
        {
            var game = _mapper.Map<Game>(request);
            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            var response = new ServiceResponse<GameCompleteDto>
            {
                Data = _mapper.Map<GameCompleteDto>(game)
            };

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
            var game = await _context.Games.FirstOrDefaultAsync(p => p.Id == gameId);

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
