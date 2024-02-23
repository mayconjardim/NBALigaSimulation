using AutoMapper;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.PlayersService
{
    public class PlayersService : IPlayersService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public PlayersService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<ServiceResponse<PlayerCompleteDto>> GetPlayerById(int playerId)
        {
            var response = new ServiceResponse<PlayerCompleteDto>();

            try
            {
                var player = await _context.Players
                    .Where(p => p.Id == playerId)
                    .Include(p => p.Team)
                    .Include(p => p.Ratings)
                    .Include(p => p.Contract)
                    .Include(p => p.RegularStats)
                    .Include(p => p.Stats)
                    .Include(p => p.PlayoffsStats)
                    .Include(p => p.PlayerAwards)
                    .FirstOrDefaultAsync();

                if (player == null) 
                {
                    response.Success = false;
                    response.Message = $"O Player com o Id {playerId} não existe!";
                }
                else
                {
                    response.Data = _mapper.Map<PlayerCompleteDto>(player);
                }
                
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Ocorreu um erro ao buscar o jogador com o ID {playerId}: {ex.Message}");
            }

            return response;
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

        public async Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllPlayers()
        {
            var players = await _context.Players.Include(p => p.Ratings).ToListAsync();
            var response = new ServiceResponse<List<PlayerCompleteDto>>
            {
                Data = _mapper.Map<List<PlayerCompleteDto>>(players)
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

        public async Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllDraftPlayers()
        {
            var players = await _context.Players.Where(t => t.TeamId == 22)
                .Include(p => p.Ratings)
                .ToListAsync();
            var response = new ServiceResponse<List<PlayerCompleteDto>>
            {
                Data = _mapper.Map<List<PlayerCompleteDto>>(players)
            };

            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateRosterOrder(List<PlayerCompleteDto> updatedPlayerList)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                for (int i = 0; i < updatedPlayerList.Count; i++)
                {
                    var player = updatedPlayerList[i];

                    Player playerDb = await _context.Players.FindAsync(player.Id);

                    if (playerDb == null)
                    {
                        response.Success = false;
                        response.Message = $"The player with ID {player.Id} does not exist!";
                        return response;
                    }

                    playerDb.RosterOrder = i;
                }

                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Roster order updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }

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

        public async Task<ServiceResponse<List<PlayerSimpleDto>>> GetPlayersSearchSuggestions(string searchText)
        {
            var response = new ServiceResponse<List<PlayerSimpleDto>>();

            var players = await _context.Players.Where(p => p.Name.ToLower().Contains(searchText.ToLower())).ToListAsync();

            response.Data = _mapper.Map<List<PlayerSimpleDto>>(players);

            response.Success = true;
            return response;
        }
    }
}
