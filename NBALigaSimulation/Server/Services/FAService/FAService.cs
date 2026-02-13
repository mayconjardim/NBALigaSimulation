using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Shared.Dtos.FA;
using NBALigaSimulation.Shared.Engine.Finance;
using NBALigaSimulation.Shared.Models.FA;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Users;
using NBALigaSimulation.Shared.Models.Utils;
using NBALigaSimulation.Server.Repositories.Interfaces;

namespace NBALigaSimulation.Server.Services.FAService
{
    public class FAService : IFAService
    {
        private const int FreeAgentsTeamId = 21;

        private readonly IGenericRepository<FAOffer> _offerRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Player> _playerRepository;
        private readonly IGenericRepository<PlayerContract> _contractRepository;
        private readonly ISeasonRepository _seasonRepository;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public FAService(
            IGenericRepository<FAOffer> offerRepository,
            IGenericRepository<User> userRepository,
            IGenericRepository<Player> playerRepository,
            IGenericRepository<PlayerContract> contractRepository,
            ISeasonRepository seasonRepository,
            IMapper mapper,
            IAuthService authService)
        {
            _offerRepository = offerRepository;
            _userRepository = userRepository;
            _playerRepository = playerRepository;
            _contractRepository = contractRepository;
            _seasonRepository = seasonRepository;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<ServiceResponse<FAOfferDto>> CreateOffer(FAOfferDto offerDto)
        {
            var response = new ServiceResponse<FAOfferDto>();
            try
            {
                var player = await _playerRepository.Query()
                    .Include(p => p.Ratings)
                    .Include(p => p.RegularStats)
                    .FirstOrDefaultAsync(p => p.Id == offerDto.PlayerId);
                if (player == null)
                {
                    response.Success = false;
                    response.Message = "Jogador não encontrado.";
                    return response;
                }
                if (player.TeamId != FreeAgentsTeamId)
                {
                    response.Success = false;
                    response.Message = "Jogador não está em free agency.";
                    return response;
                }

                int exp = player.Ratings?.Count ?? 0;
                int minAmount = SalaryCapConstants.GetMinSalaryForFreeAgent(exp);
                int maxAmount = SalaryCapConstants.GetMaxSalaryForFreeAgent(exp);
                var lastStat = player.RegularStats?.OrderByDescending(s => s.Season).FirstOrDefault();
                bool isCurrentTeam = lastStat != null && lastStat.TeamId == offerDto.TeamId;
                int maxYears = SalaryCapConstants.GetMaxContractYears(isCurrentTeam);

                if (offerDto.Amount < minAmount || offerDto.Amount > maxAmount)
                {
                    response.Success = false;
                    response.Message = $"Valor deve estar entre {minAmount:N0} e {maxAmount:N0} (EXP {exp}).";
                    return response;
                }
                if (offerDto.Years < 1 || offerDto.Years > maxYears)
                {
                    response.Success = false;
                    response.Message = $"Anos devem ser entre 1 e {maxYears}.";
                    return response;
                }

                var teamPlayers = await _playerRepository.Query()
                    .Where(p => p.TeamId == offerDto.TeamId)
                    .Include(p => p.Contract)
                    .ToListAsync();
                int teamSalary = teamPlayers.Where(p => p.Contract != null).Sum(p => p.Contract!.Amount);
                if (teamSalary + offerDto.Amount > SalaryCapConstants.SalaryCap)
                {
                    response.Success = false;
                    response.Message = "O time não tem cap space para esta oferta.";
                    return response;
                }

                bool duplicate = await _offerRepository.Query()
                    .AnyAsync(o => o.TeamId == offerDto.TeamId && o.PlayerId == offerDto.PlayerId && o.Season == offerDto.Season && o.Response == null);
                if (duplicate)
                {
                    response.Success = false;
                    response.Message = "Já existe uma oferta pendente deste time para este jogador nesta temporada.";
                    return response;
                }

                var offer = _mapper.Map<FAOffer>(offerDto);
                await _offerRepository.AddAsync(offer);
                await _offerRepository.SaveChangesAsync();
                response.Success = true;
                response.Data = _mapper.Map<FAOfferDto>(offer);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocorreu um erro ao criar uma oferta: " + ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<FAOfferDto>>> GetOffersByTeamId()
        {
            var response = new ServiceResponse<List<FAOfferDto>>();

            var userId = _authService.GetUserId();
            var user = await _userRepository.Query()
                .FirstOrDefaultAsync(u => u.Id == userId);
            int? teamId = null;

            if (user != null)
            {
                teamId = user.TeamId;
            }

            List<FAOffer> offers = await _offerRepository.Query()
                .Include(p => p.Player)
                .OrderByDescending(o => o.DateCreated)
                .Where(o => o.TeamId == teamId)
                .ToListAsync();

            if (offers == null)
            {
                response.Success = false;
                response.Message = $"As ofertas com time de Id {teamId} não existem!";
            }
            else
            {
                response.Data = _mapper.Map<List<FAOfferDto>>(offers);
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteOffer(int offerId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var offer = await _offerRepository.GetByIdAsync(offerId);
                if (offer == null)
                {
                    response.Success = false;
                    response.Message = "A oferta não foi encontrada.";
                    return response;
                }
                _offerRepository.Remove(offer);
                await _offerRepository.SaveChangesAsync();
                response.Success = true;
                response.Data = true;
                response.Message = "A oferta foi excluída com sucesso.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocorreu um erro ao excluir a oferta: " + ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<FASimulateRoundResultDto>> SimulateFARound(int? seasonYear = null)
        {
            var result = new ServiceResponse<FASimulateRoundResultDto> { Data = new FASimulateRoundResultDto() };
            try
            {
                int year = seasonYear ?? (await _seasonRepository.GetLastSeasonAsync())?.Year ?? DateTime.Now.Year;

                var pending = await _offerRepository.Query()
                    .Where(o => o.Season == year && o.Response == null)
                    .Include(o => o.Player)
                    .ThenInclude(p => p.Contract)
                    .Include(o => o.Team)
                    .ToListAsync();

                var byPlayer = pending.GroupBy(o => o.PlayerId).ToList();
                var signings = new List<FASigningDto>();

                foreach (var group in byPlayer)
                {
                    var best = group.OrderByDescending(o => o.Amount).ThenByDescending(o => o.Years).First();
                    best.Response = true;
                    foreach (var other in group.Where(o => o.Id != best.Id))
                        other.Response = false;

                    var player = best.Player;
                    if (player == null) continue;

                    int exp = best.Season + best.Years;
                    if (player.Contract != null)
                    {
                        player.Contract.Amount = best.Amount;
                        player.Contract.Exp = exp;
                    }
                    else
                    {
                        var contract = new PlayerContract
                        {
                            PlayerId = player.Id,
                            Amount = best.Amount,
                            Exp = exp
                        };
                        await _contractRepository.AddAsync(contract);
                        player.Contract = contract;
                    }
                    player.TeamId = best.TeamId;

                    signings.Add(new FASigningDto
                    {
                        PlayerName = player.Name,
                        TeamName = best.Team?.Name ?? "",
                        Amount = best.Amount,
                        Years = best.Years
                    });
                }

                await _offerRepository.SaveChangesAsync();
                await _playerRepository.SaveChangesAsync();
                await _contractRepository.SaveChangesAsync();

                result.Data.SigningsCount = signings.Count;
                result.Data.Signings = signings;
                result.Data.Message = signings.Count > 0
                    ? $"{signings.Count} jogador(es) assinaram na rodada."
                    : "Nenhuma oferta pendente para processar.";
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Erro ao simular rodada da FA: " + ex.Message;
            }
            return result;
        }
    }
}
