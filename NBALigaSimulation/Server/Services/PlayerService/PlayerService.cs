using AutoMapper;
using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Server.Services.PlayerService
{
    public class PlayerService : IPlayerService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public PlayerService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<PlayerSimpleDto>>> GetAllPlayers()
        {
            var players = await _context.Players.ToListAsync();
            var response = new ServiceResponse<List<PlayerSimpleDto>>
            {
                Data = _mapper.Map<List<PlayerSimpleDto>>(players)
            };

            return response;
        }

        public async Task<ServiceResponse<PlayerCompleteDto>> GetPlayerById(int playerId)
        {
            var response = new ServiceResponse<PlayerCompleteDto>();
            var player = await _context.Players.Include(p => p.Ratings).FirstOrDefaultAsync(p => p.Id == playerId);

            if (player == null)
            {
                response.Success = false;
                response.Message = $"O Player com o Id {playerId} não existe!";
            }
            else
            {
                response.Data = _mapper.Map<PlayerCompleteDto>(player);
            }

            return response;
        }
    }
}
