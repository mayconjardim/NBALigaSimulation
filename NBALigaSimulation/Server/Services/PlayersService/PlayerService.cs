using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Teams;
using NBALigaSimulation.Shared.Models.Utils;
using Player = NBALigaSimulation.Shared.Models.Players.Player;
using NBALigaSimulation.Server.Extensions;

namespace NBALigaSimulation.Server.Services.PlayersService
{
    public class PlayerService : IPlayerService
    {
        private readonly IGenericRepository<Player> _playerRepository;
        private readonly IMapper _mapper;

        public PlayerService(
            IGenericRepository<Player> playerRepository,
            IMapper mapper)
        {
            _playerRepository = playerRepository;
            _mapper = mapper;
        }
        
        public async Task<ServiceResponse<PlayerCompleteDto>> GetPlayerById(int playerId)
        {
            var response = new ServiceResponse<PlayerCompleteDto>();

            try
            {
                var player = await _playerRepository.Query()
                    .Where(p => p.Id == playerId)
                    .Include(p => p.Team)
                    .Include(p => p.Ratings)
                    .Include(p => p.Contract)
                    .Include(p => p.RegularStats)
                    .Include(p => p.Stats).ThenInclude(s => s.Game)
                    .Include(p => p.PlayoffsStats)
                    .Include(p => p.PlayerAwards)
                    .Include(p => p.AwardCounts)
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

        public async Task<ServiceResponse<List<PlayerSimpleDto>>> GetAllSimplePlayers()
        {
            var players = await _playerRepository.GetAllAsync();
            var response = new ServiceResponse<List<PlayerSimpleDto>>
            {
                Data = _mapper.Map<List<PlayerSimpleDto>>(players)
            };

            return response;
        }
        
        public async Task<ServiceResponse<PageableResponse<PlayerCompleteDto>>> GetAllFaPlayers(int page, int pageSize, int season, bool isAscending, string sortedColumn,
            string position = null)
        {
          
              var response = new ServiceResponse<PageableResponse<PlayerCompleteDto>>();
		    try 
            {
                
                IQueryable<Player> query = _playerRepository.Query()
                    .Where(p => p.TeamId == TeamConstants.FreeAgents)
                    .Include(p => p.Ratings);

                List<Player> players = string.IsNullOrEmpty(position) || position == "ALL"
                    ? query.ToList()
                    : query.Where(p => p.Pos == position).ToList();

                var playersFiltered = players
                    .ApplySorting(sortedColumn, isAscending)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

		        var totalPages = (int)Math.Ceiling(await query.CountAsync() / (double)pageSize);

		        var playerFAtoList = _mapper.Map<List<PlayerCompleteDto>>(playersFiltered);

		        var playerFaResponse = new PageableResponse<PlayerCompleteDto>()
		        {
		            Response = playerFAtoList,
		            Pages = totalPages,
		            CurrentPage = page
		        };

		        response.Data = playerFaResponse;
		        response.Success = true;
		    }
		    catch (Exception ex)
		    {
		        response.Success = false;
		        response.Message = $"Ocorreu um erro ao recuperar os jogadores: {ex.Message}";
		    }

		    return response; 
            
        }

        public async Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllDraftPlayers()
        {
            var players = await _playerRepository.Query()
                .Where(t => t.TeamId == TeamConstants.Draft)
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

                    Player playerDb = await _playerRepository.GetByIdAsync(player.Id);

                    if (playerDb == null)
                    {
                        response.Success = false;
                        response.Message = $"O Jogador com o ID {player.Id} não existe!";
                        return response;
                    }

                    playerDb.RosterOrder = i;
                }

                await _playerRepository.SaveChangesAsync();

                response.Success = true;
                response.Message = "Ordem de rotação atualizada com sucesso.";
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

            var player = await _playerRepository.Query()
                .Include(t => t.Team)
                .FirstOrDefaultAsync(p => p.Id == playerId);

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

            await _playerRepository.SaveChangesAsync();
            response.Success = true;
            response.Message = "Modificador de pontos (PtModifier) atualizado com sucesso.";

            return response;
        }

        public async Task<ServiceResponse<PlayerCompleteDto>> EditPlayer(CreatePlayerDto playerDto)
        {
            var player = await _playerRepository.Query()
                .Include(p => p.Ratings) 
                .Include(p => p.Born)    
                .FirstOrDefaultAsync(p => p.Id == playerDto.Id);

            if (player == null)
            {
                return new ServiceResponse<PlayerCompleteDto>
                {
                    Success = false,
                    Message = "Jogador não encontrado."
                };
            }

            _mapper.Map(playerDto, player);

            try
            {
                await _playerRepository.SaveChangesAsync();

                var updatedPlayer = _mapper.Map<PlayerCompleteDto>(player);
                return new ServiceResponse<PlayerCompleteDto>
                {
                    Success = true,
                    Data = updatedPlayer,
                    Message = "Jogador atualizado com sucesso."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PlayerCompleteDto>
                {
                    Success = false,
                    Message = $"Erro ao atualizar o jogador: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<List<PlayerSimpleDto>>> GetPlayersSearchSuggestions(string searchText)
        {
            var response = new ServiceResponse<List<PlayerSimpleDto>>();

            List<Player> cleanList = new List<Player>();
            
            if (string.IsNullOrEmpty(searchText))
            {
                response.Data = _mapper.Map<List<PlayerSimpleDto>>(cleanList);
                return response;  
            }
            
            var players = await _playerRepository.Query()
                .Where(p => EF.Functions.Like(p.Name, $"%{searchText}%"))
                .ToListAsync();
            response.Data = _mapper.Map<List<PlayerSimpleDto>>(players);
            return response;  
           
        }
    }
}
