using AutoMapper;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Utils;
using Radzen;

namespace NBALigaSimulation.Server.Services.PlayersService
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
        
        public async Task<ServiceResponse<List<PlayerSimpleDto>>> GetAllSimplePlayers()
        {
            var players = await _context.Players.ToListAsync();
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
                
                IQueryable<Player> query = _context.Players
                    .Where(p => p.TeamId == 21)
                    .Include(p => p.Ratings);

                List<Player> players;
                
                if (!string.IsNullOrEmpty(position))
                {
                    if (position == "ALL")
                    {
                        players = query.ToList(); 
                    }
                    else
                    {
                        players = query.Where(p => p.Pos == position).ToList(); 
                    }
                }
                else
                {
                    players = query.ToList(); 
                }

                var orderByExpression = players.OrderBy(p => p.Ratings.LastOrDefault()?.CalculateOvr);

                if (!string.IsNullOrEmpty(sortedColumn))
                {
                    orderByExpression = sortedColumn switch
                    {
                        "OVR" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.CalculateOvr) : players.OrderBy(p => p.Ratings.LastOrDefault()?.CalculateOvr),
                        "POT" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Pot) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Pot),
                        "HGT" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Hgt) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Hgt),
                        "STR" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Stre) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Stre),
                        "SPD" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Spd) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Spd),
                        "JMP" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Jmp): players.OrderBy(p => p.Ratings.LastOrDefault()?.Jmp),
                        "END" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Endu) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Endu),
                        "INS" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Ins) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Ins),
                        "DNK" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Dnk) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Dnk),
                        "FT" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Ft) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Ft),
                        "2PT" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Fg) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Fg),
                        "3PT" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Tp) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Tp),
                        "OIQ" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Oiq) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Oiq),
                        "DIQ" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Diq) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Diq),
                        "DRB" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Drb) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Drb),
                        "PSS" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Pss) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Pss),
                        "REB" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Reb) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Reb),
                        _ => orderByExpression
                    };
                }

                var playersFiltered = orderByExpression
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

        public async Task<ServiceResponse<PlayerCompleteDto>> EditPlayer(CreatePlayerDto playerDto)
        {
            var player = await _context.Players
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
                await _context.SaveChangesAsync();

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
            
            var players = await _context.Players.Where(p => EF.Functions.Like(p.Name, $"%{searchText}%")).ToListAsync();
            response.Data = _mapper.Map<List<PlayerSimpleDto>>(players);
            return response;  
           
        }
        
    }
}
