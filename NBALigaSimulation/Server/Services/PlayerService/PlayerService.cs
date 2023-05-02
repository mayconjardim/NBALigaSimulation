namespace NBALigaSimulation.Server.Services.PlayerService
{
    public class PlayerService : IPlayerService
    {
        private readonly DataContext _context;

        public PlayerService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<Player>>> GetAllPlayers()
        {
            var response = new ServiceResponse<List<Player>>
            {
                Data = await _context.Players.ToListAsync()
            };

            return response;
        }

        public async Task<ServiceResponse<Player>> GetPlayerById(int playerId)
        {
            var response = new ServiceResponse<Player>();
            var player = await _context.Players.Include(p => p.Ratings).FirstOrDefaultAsync(p => p.Id == playerId);

            if (player == null)
            {
                response.Success = false;
                response.Message = $"O Player com o Id {playerId} não existe!";
            }
            else
            {
                response.Data = player;
            }

            return response;
        }
    }
}
