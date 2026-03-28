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
    public class PlayerContractService : IPlayerContractService
    {
        private readonly IGenericRepository<Player> _playerRepository;
        private readonly ISeasonRepository _seasonRepository;
        private readonly IMapper _mapper;

        public PlayerContractService(
            IGenericRepository<Player> playerRepository,
            ISeasonRepository seasonRepository,
            IMapper mapper)
        {
            _playerRepository = playerRepository;
            _seasonRepository = seasonRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<bool>> GenerateContracts()
        {
            var response = new ServiceResponse<bool>();

            List<Player> players = await _playerRepository.Query()
                .OrderBy(p => p.Id)
                .Include(p => p.Ratings)
                .ToListAsync();

            Season season = await _seasonRepository.Query()
                .OrderBy(s => s.Id)
                .LastOrDefaultAsync();

            foreach (Player player in players)
            {
                var contract = ArrayHelper.GenContract(player, season, true, true, false);
                player.Contract = contract;
            }

            await _playerRepository.SaveChangesAsync();
            response.Success = true;
            response.Message = "Contratos gerados com sucesso.";

            return response;
        }

        public async Task<ServiceResponse<bool>> GenerateContractsForTest()
        {
            var response = new ServiceResponse<bool>();

            var season = await _seasonRepository.Query()
                .OrderBy(s => s.Id)
                .LastOrDefaultAsync();
            if (season == null)
            {
                response.Success = false;
                response.Message = "Temporada não encontrada.";
                return response;
            }

            var players = await _playerRepository.Query()
                .Include(p => p.Ratings)
                .Include(p => p.Contract)
                .Where(p => p.TeamId != 21 && p.TeamId != 22)
                .ToListAsync();

            ContractTestUtils.AssignContractsForTest(players, season.Year);

            await _playerRepository.SaveChangesAsync();
            response.Success = true;
            response.Message = $"Contratos gerados para {players.Count} jogadores, respeitando o cap.";
            return response;
        }

        public async Task<ServiceResponse<bool>> HandleExpiredContractsAsync(int nextSeasonYear)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var expiredContractPlayers = await _playerRepository.Query()
                    .Include(p => p.Contract)
                    .Where(p => p.TeamId != GlobalConstants.FreeAgentsTeamId && p.Contract != null && p.Contract.Exp < nextSeasonYear)
                    .ToListAsync();

                foreach (var player in expiredContractPlayers)
                {
                    player.TeamId = GlobalConstants.FreeAgentsTeamId;
                }

                if (expiredContractPlayers.Count > 0)
                {
                    await _playerRepository.SaveChangesAsync();
                }

                response.Success = true;
                response.Message = $"{expiredContractPlayers.Count} jogadores com contratos expirados movidos para a agência livre.";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro ao processar contratos expirados: {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<List<PlayerCompleteDto>>> GetExpiringPlayers(int season)
        {
            var response = new ServiceResponse<List<PlayerCompleteDto>>();
            try
            {
                var players = await _playerRepository.Query()
                    .Where(p => p.TeamId != 21 && p.TeamId != 22 && p.Contract != null && p.Contract.Exp == season)
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
                response.Message = $"Erro ao buscar jogadores com contrato expirando: {ex.Message}";
            }

            return response;
        }
    }
}
