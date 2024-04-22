using AutoMapper;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Utils;

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
        
        public async Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllFaPlayers()
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

        public async Task<ServiceResponse<PageableResponse<PlayerCompleteDto>>> GetAllFaPlayers(int page, int pageSize, int season, bool isAscending, string sortedColumn,
            string position = null)
        {
          
              var response = new ServiceResponse<PageableResponse<PlayerCompleteDto>>();

		    try
            {
                
                IQueryable<Player> query = _context.Players.Where(t => t.TeamId == 21)
                    .Include(p => p.Ratings);

                var orderByExpression = query.OrderByDescending(p =>
                    p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().CalculateOvr);
                
                if (!string.IsNullOrEmpty(sortedColumn))
                {
                    switch (sortedColumn)
                    {
                        case "OVR":
                            orderByExpression = isAscending ?
                                query.OrderByDescending(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().CalculateOvr) :
                                query.OrderBy(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().CalculateOvr);
                            break;
                        case "POT":
                            orderByExpression = isAscending ?
                                query.OrderByDescending(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Pot) :
                                query.OrderBy(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Pot);
                            break;
                        case "HGT":
                            orderByExpression = isAscending ?
                                query.OrderByDescending(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Hgt) :
                                query.OrderBy(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Hgt);
                            break;
                        case "STR":
                            orderByExpression = isAscending ?
                                query.OrderByDescending(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Stre) :
                                query.OrderBy(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Stre);
                            break;
                        case "SPD":
                            orderByExpression = isAscending ?
                                query.OrderByDescending(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Spd) :
                                query.OrderBy(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Spd);
                            break;
                        case "JMP":
                            orderByExpression = isAscending ?
                                query.OrderByDescending(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Jmp) :
                                query.OrderBy(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Jmp);
                            break;
                        case "END":
                            orderByExpression = isAscending ?
                                query.OrderByDescending(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Endu) :
                                query.OrderBy(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Endu);
                            break;
                        case "INS":
                            orderByExpression = isAscending ?
                                query.OrderByDescending(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Ins) :
                                query.OrderBy(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Ins);
                            break;
                        case "DNK":
                            orderByExpression = isAscending ?
                                query.OrderByDescending(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Dnk) :
                                query.OrderBy(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Dnk);
                            break;
                        case "FT":
                            orderByExpression = isAscending ?
                                query.OrderByDescending(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Ft) :
                                query.OrderBy(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Ft);
                            break;
                        case "2PT":
                            orderByExpression = isAscending ?
                                query.OrderByDescending(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Fg) :
                                query.OrderBy(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Fg);
                            break;
                        case "3PT":
                            orderByExpression = isAscending ?
                                query.OrderByDescending(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Tp) :
                                query.OrderBy(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Tp);
                            break;
                        case "OIQ":
                            orderByExpression = isAscending ?
                                query.OrderByDescending(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Oiq) :
                                query.OrderBy(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Oiq);
                            break;
                        case "DIQ":
                            orderByExpression = isAscending ?
                                query.OrderByDescending(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Diq) :
                                query.OrderBy(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Diq);
                            break;
                        case "DRB":
                            orderByExpression = isAscending ?
                                query.OrderByDescending(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Drb) :
                                query.OrderBy(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Drb);
                            break;
                        case "PSS":
                            orderByExpression = isAscending ?
                                query.OrderByDescending(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Pss) :
                                query.OrderBy(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Pss);
                            break;
                        case "REB":
                            orderByExpression = isAscending ?
                                query.OrderByDescending(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Reb) :
                                query.OrderBy(p => p.Ratings.OrderByDescending(r => r.Season == season).FirstOrDefault().Reb);
                            break;
                        default:
                            break;
                    }
                }

		        
		        if (!string.IsNullOrEmpty(position))
		        {
			        query = orderByExpression.Where(p => p.Pos == position);
		        }

		        var players = await query
		            .Skip((page - 1) * pageSize)
		            .Take(pageSize)
		            .ToListAsync();

		        var totalPages = (int)Math.Ceiling(await query.CountAsync() / (double)pageSize);

		        var playerFAtoList = _mapper.Map<List<PlayerCompleteDto>>(players);

		        var playerFAResponse = new PageableResponse<PlayerCompleteDto>()
		        {
		            Response = playerFAtoList,
		            Pages = totalPages,
		            CurrentPage = page
		        };

		        response.Data = playerFAResponse;
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
