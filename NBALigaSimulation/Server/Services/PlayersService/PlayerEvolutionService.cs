using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Utils;
using Player = NBALigaSimulation.Shared.Models.Players.Player;

namespace NBALigaSimulation.Server.Services.PlayersService
{
    public class PlayerEvolutionService : IPlayerEvolutionService
    {
        private readonly IGenericRepository<Player> _playerRepository;
        private readonly ISeasonRepository _seasonRepository;
        private readonly IMapper _mapper;

        public PlayerEvolutionService(
            IGenericRepository<Player> playerRepository,
            ISeasonRepository seasonRepository,
            IMapper mapper)
        {
            _playerRepository = playerRepository;
            _seasonRepository = seasonRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<bool>> GenerateTrainingCampAsync(Season season)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                List<Player> players = await _seasonRepository.GetPlayersWithRatingsAsync();

                if (players.Any(p => p.Ratings.Where(s => s.Season == season.Year).Any()))
                {
                    response.Success = false;
                    response.Message = "TrainingCamp já realizado!";
                    return response;
                }

                foreach (Player player in players)
                {
                    var newRatings = TcUtils.TrainingCamp(player, season);
                    newRatings.ScoutReport = NBALigaSimulation.Shared.Engine.Scouting.ScoutingReportGenerator.Generate(newRatings, player.Pos ?? "SF", player.Born?.Year);
                    player.Ratings.Add(newRatings);
                }

                await _seasonRepository.SaveChangesAsync();

                response.Success = true;
                response.Message = "TrainingCamp gerado com sucesso para todos os jogadores!";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro ao gerar TrainingCamp: {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<bool>> HandleOffSeasonRecoveryAsync(int healingAmount)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                List<Player> players = await _seasonRepository.GetPlayersWithRatingsAsync();
                
                SimulationUtils.ApplyOffSeasonInjuryRecovery(players, healingAmount);
                
                await _seasonRepository.SaveChangesAsync();
                
                response.Success = true;
                response.Message = $"Recuperação de off-season aplicada a {players.Count} jogadores.";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro ao aplicar recuperação de off-season: {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<bool>> RegenerateAllScoutReports()
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var players = await _playerRepository.Query()
                    .Include(p => p.Ratings)
                    .ToListAsync();

                int ratingsCount = 0;
                foreach (var player in players)
                {
                    var pos = string.IsNullOrWhiteSpace(player.Pos) ? "SF" : player.Pos;
                    foreach (var rating in player.Ratings ?? new List<PlayerRatings>())
                    {
                        rating.ScoutReport = NBALigaSimulation.Shared.Engine.Scouting.ScoutingReportGenerator.Generate(rating, pos, player.Born?.Year);
                        ratingsCount++;
                    }
                }

                await _playerRepository.SaveChangesAsync();
                response.Success = true;
                response.Data = true;
                response.Message = $"Scout reports gerados para {players.Count} jogador(es) ({ratingsCount} rating(s)).";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = false;
                response.Message = $"Erro ao gerar scout reports: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<List<PlayerCompleteDto>>> GetInjuredPlayers()
        {
            var response = new ServiceResponse<List<PlayerCompleteDto>>();
            try
            {
                var players = await _playerRepository.Query()
                    .Where(p => !string.IsNullOrEmpty(p.InjuryType) && (p.InjuryGamesRemaining ?? 0) > 0)
                    .Include(p => p.Team)
                    .Include(p => p.Ratings)
                    .Include(p => p.Contract)
                    .ToListAsync();

                var ordered = players
                    .OrderBy(p => p.Team?.Name)
                    .ThenByDescending(p => p.Ratings.OrderByDescending(r => r.Season).FirstOrDefault()?.CalculateOvr ?? 0)
                    .ThenBy(p => p.Name)
                    .ToList();

                response.Data = _mapper.Map<List<PlayerCompleteDto>>(ordered);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro ao buscar jogadores lesionados: {ex.Message}";
            }

            return response;
        }
    }
}
