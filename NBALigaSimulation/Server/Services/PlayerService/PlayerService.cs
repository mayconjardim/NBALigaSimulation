using AutoMapper;

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

        public async Task<ServiceResponse<PlayerCompleteDto>> CreatePlayer(CreatePlayerDto request)
        {

            ServiceResponse<PlayerCompleteDto> response = new ServiceResponse<PlayerCompleteDto>();

            Player player = _mapper.Map<Player>(request);

            _context.Add(player);

            await _context.SaveChangesAsync();

            response.Success = true;
            response.Data = _mapper.Map<PlayerCompleteDto>(player);
            return response;
        }

        public async Task<ServiceResponse<List<PlayerCompleteDto>>> CreatePlayers(List<CreatePlayerDto> requests)
        {
            ServiceResponse<List<PlayerCompleteDto>> response = new ServiceResponse<List<PlayerCompleteDto>>();

            List<Player> players = _mapper.Map<List<Player>>(requests);

            _context.AddRange(players);

            await _context.SaveChangesAsync();

            response.Success = true;
            response.Data = _mapper.Map<List<PlayerCompleteDto>>(players);
            return response;
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

        public async Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllFAPlayers()
        {
            var players = await _context.Players.Where(t => t.TeamId == 21)
                .Include(p => p.Ratings)
                .ToListAsync();
            var response = new ServiceResponse<List<PlayerCompleteDto>>
            {
                Data = _mapper.Map<List<PlayerCompleteDto>>(players)
            };

            return response;
        }

        public async Task<ServiceResponse<PlayerCompleteDto>> GetPlayerById(int playerId)
        {
            var response = new ServiceResponse<PlayerCompleteDto>();
            var player = await _context.Players
                .Include(t => t.Team)
                .Include(p => p.Ratings)
                .Include(p => p.Contract)
                .Include(p => p.RegularStats)
                .Include(p => p.Stats)
                .FirstOrDefaultAsync(p => p.Id == playerId);

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

        public async Task<ServiceResponse<bool>> UpdateRosterOrder(List<PlayerCompleteDto> updatedPlayerList)
        {
            var response = new ServiceResponse<bool>();

            foreach (var player in updatedPlayerList)
            {
                var playerDb = await _context.Players.FirstOrDefaultAsync(p => p.Id == player.Id);

                if (playerDb == null)
                {
                    response.Success = false;
                    response.Message = $"O Player com o Id {player.Id} não existe!";
                    return response;

                }
                else
                {
                    int rosterOrder = updatedPlayerList.IndexOf(player);
                    playerDb.RosterOrder = rosterOrder;
                }

            }

            await _context.SaveChangesAsync();
            response.Success = true;
            response.Message = "Roster order updated successfully.";

            return response;
        }

        public async Task<ServiceResponse<bool>> UpdatePlayerPtModifier(int playerId, double newPtModifier)
        {
            var response = new ServiceResponse<bool>();

            var player = await _context.Players.Include(t => t.Team).FirstOrDefaultAsync(p => p.Id == playerId);

            if (player == null)
            {
                response.Success = false;
                response.Message = $"O Player com o Id {playerId} não existe!";
                return response;
            }
            else
            {
                player.PtModifier = newPtModifier;
            }

            await _context.SaveChangesAsync();
            response.Success = true;
            response.Message = "PtModifier updated successfully.";

            return response;
        }

        public async Task<ServiceResponse<bool>> GenerateContracts()
        {
            var response = new ServiceResponse<bool>();

            List<Player> players = await _context.Players.OrderBy(p => p.Id).Include(p => p.Ratings).ToListAsync();

            Season season = await _context.Seasons.OrderBy(s => s.Id).LastOrDefaultAsync();

            foreach (Player player in players)
            {
                var contract = ArrayHelper.GenContract(player, season, true, true, false);
                player.Contract = contract;
            }

            await _context.SaveChangesAsync();
            response.Success = true;
            response.Message = "Contracts generated successfully.";

            return response;
        }

    }
}
