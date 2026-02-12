using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Dtos.Drafts;
using NBALigaSimulation.Shared.Models.Drafts;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.DraftService
{
    public class DraftService : IDraftService
    {

        private readonly IGenericRepository<DraftLottery> _draftLotteryRepository;
        private readonly IGenericRepository<Draft> _draftRepository;
        private readonly IGenericRepository<Season> _seasonRepository;
        private readonly IGenericRepository<TeamRegularStats> _teamRegularStatsRepository;
        private readonly IGenericRepository<Team> _teamRepository;
        private readonly IGenericRepository<TeamDraftPicks> _draftPickRepository;
        private readonly IGenericRepository<Player> _playerRepository;
        private readonly IMapper _mapper;

        public DraftService(
            IGenericRepository<DraftLottery> draftLotteryRepository,
            IGenericRepository<Draft> draftRepository,
            IGenericRepository<Season> seasonRepository,
            IGenericRepository<TeamRegularStats> teamRegularStatsRepository,
            IGenericRepository<Team> teamRepository,
            IGenericRepository<TeamDraftPicks> draftPickRepository,
            IGenericRepository<Player> playerRepository,
            IMapper mapper)
        {
            _draftLotteryRepository = draftLotteryRepository;
            _draftRepository = draftRepository;
            _seasonRepository = seasonRepository;
            _teamRegularStatsRepository = teamRegularStatsRepository;
            _teamRepository = teamRepository;
            _draftPickRepository = draftPickRepository;
            _playerRepository = playerRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<DraftLotteryDto>> GetLastLottery()
        {
            var response = new ServiceResponse<DraftLotteryDto>();
            var lottery = await _draftLotteryRepository.Query()
                .OrderBy(s => s.Season)
                .LastOrDefaultAsync();

            if (lottery == null)
            {
                response.Success = false;
                response.Message = $"Loteria não econtrada!";
            }
            else
            {
                response.Data = _mapper.Map<DraftLotteryDto>(lottery);
            }

            return response;
        }


        public async Task<ServiceResponse<List<DraftDto>>> GetLastDraft()
        {
            var response = new ServiceResponse<List<DraftDto>>();
            var season = await _seasonRepository.Query()
                .OrderBy(s => s.Year)
                .LastOrDefaultAsync();
            var draft = await _draftRepository.Query()
                .OrderBy(s => s.Pick)
                .Where(d => d.Season == season.Year)
                .Include(d => d.Team)
                .Include(d => d.Player)
                .ToListAsync();

            if (draft == null)
            {
                response.Success = false;
                response.Message = $"Draft não econtrado!";
            }
            else
            {
                response.Data = _mapper.Map<List<DraftDto>>(draft);
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> GenerateLottery()
        {
            var response = new ServiceResponse<bool>();
            var season = await _seasonRepository.Query()
                .OrderBy(s => s.Year)
                .LastOrDefaultAsync();

            if (season == null)
            {
                response.Success = false;
                response.Message = "Nenhuma temporada encontrada.";
                return response;
            }

            if (!season.IsCompleted)
            {
                response.Success = false;
                response.Message = "Não é possível gerar a loteria. Finalize os playoffs da temporada atual primeiro.";
                return response;
            }

            var existingLottery = await _draftLotteryRepository.Query()
                .Where(l => l.Season == season.Year)
                .OrderByDescending(l => l.Id)
                .FirstOrDefaultAsync();

            if (existingLottery != null)
            {
                response.Success = false;
                response.Message = "A loteria do draft desta temporada já foi gerada.";
                return response;
            }

            var teams = await _teamRegularStatsRepository.Query()
                .Where(t => t.Season == season.Year)
                .Include(t => t.Team)
                .ToListAsync();

            if (teams.Count < 4)
            {
                response.Success = false;
                response.Message = $"É necessário ter estatísticas da temporada regular (pelo menos 4 times que não foram aos playoffs). Encontrados: {teams.Count}.";
                return response;
            }

            // Os 4 piores (que não foram aos playoffs) entram na loteria
            teams = teams.AsEnumerable().OrderByDescending(t => t.ConfRank).Take(4).ToList();
            teams = teams.OrderByDescending(t => t.WinPct).ToList();

            var order = DraftUtils.RunLottery(teams);

            var newLottery = new DraftLottery
            {
                Season = season.Year,
                FirstTeam = order[0].Team.Abrv,
                FirstTeamId = order[0].TeamId,
                SecondTeam = order[1].Team.Abrv,
                SecondTeamId = order[1].TeamId,
                ThirdTeam = order[2].Team.Abrv,
                ThirdTeamId = order[2].TeamId,
                FourthTeam = order[3].Team.Abrv,
                FourthTeamId = order[3].TeamId,
                FifthTeam = order[3].Team.Abrv,
                FifthTeamId = order[3].TeamId,
                SixthTeam = order[3].Team.Abrv,
                SixthTeamId = order[3].TeamId,
            };

            await _draftLotteryRepository.AddAsync(newLottery);
            await _draftLotteryRepository.SaveChangesAsync();

            // Marca flag na temporada
            season.LotteryCompleted = true;
            await _seasonRepository.SaveChangesAsync();

            response.Message = "Loteria criada com sucesso!";
            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<bool>> GenerateDraft()
        {
            var response = new ServiceResponse<bool>();
            var season = await _seasonRepository.Query()
                .OrderBy(s => s.Year)
                .LastOrDefaultAsync();

            if (season == null)
            {
                response.Success = false;
                response.Message = "Nenhuma temporada encontrada.";
                return response;
            }

            if (!season.IsCompleted)
            {
                response.Success = false;
                response.Message = "Não é possível criar o draft. Finalize os playoffs da temporada atual primeiro.";
                return response;
            }

            if (!season.LotteryCompleted)
            {
                response.Success = false;
                response.Message = "Gere a loteria do draft antes de gerar o draft.";
                return response;
            }

            var lottery = await _draftLotteryRepository.Query()
                .Where(l => l.Season == season.Year)
                .OrderByDescending(l => l.Id)
                .FirstOrDefaultAsync();

            if (lottery == null)
            {
                response.Success = false;
                response.Message = "Loteria não encontrada para esta temporada. Gere a loteria do draft primeiro.";
                return response;
            }

            var newDraft = await _draftRepository.Query()
            .Where(l => l.Season == season.Year)
            .OrderByDescending(l => l.Id).ToListAsync();

            if (newDraft.Count > 0)
            {
                response.Success = false;
                response.Message = "Draft já criado!";
                return response;
            }

            var teams = await _teamRepository.Query()
                .Where(t => t.IsHuman == true)
                .ToListAsync();

            var regularStats = await _teamRegularStatsRepository.Query()
                .Where(s => s.Season == season.Year)
                .Include(t => t.Team)
                .ToListAsync();

            var picks = await _draftPickRepository.Query()
                .Where(s => s.Year == season.Year)
                .Include(t => t.Team)
                .ToListAsync();

            newDraft = DraftUtils.GenerateDraft(lottery, season.Year, regularStats, picks, teams);

            await _draftRepository.AddRangeAsync(newDraft);

            await _draftRepository.SaveChangesAsync();

            season.DraftCompleted = true;
            await _seasonRepository.SaveChangesAsync();

            response.Message = "Draft criado com sucesso!";
            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<bool>> SelectDraftedPlayer(DraftPlayerDto request)
        {
            Console.WriteLine($"[BACKEND] SelectDraftedPlayer iniciado - PlayerId: {request?.PlayerId}, TeamId: {request?.TeamId}, Pick: {request?.Pick}");

            var response = new ServiceResponse<bool>();
            
            try
            {
                Console.WriteLine("[BACKEND] Buscando temporada...");
                var season = await _seasonRepository.Query()
                    .OrderBy(s => s.Year)
                    .LastOrDefaultAsync();

                if (season == null)
                {
                    Console.WriteLine("[BACKEND] ERRO: Temporada não encontrada");
                    response.Success = false;
                    response.Message = "Nenhuma temporada encontrada.";
                    return response;
                }
                Console.WriteLine($"[BACKEND] Temporada encontrada: {season.Year}");

                Console.WriteLine($"[BACKEND] Buscando jogador com ID: {request.PlayerId}");
                var player = await _playerRepository.Query()
                    .Where(p => p.Id == request.PlayerId)
                    .FirstOrDefaultAsync();

                if (player == null)
                {
                    Console.WriteLine($"[BACKEND] ERRO: Jogador não encontrado - ID: {request.PlayerId}");
                    response.Success = false;
                    response.Message = "Jogador não encontrado.";
                    return response;
                }
                Console.WriteLine($"[BACKEND] Jogador encontrado: {player.Name} (ID: {player.Id})");

                Console.WriteLine($"[BACKEND] Buscando draft - Pick: {request.Pick}, TeamId: {request.TeamId}, Season: {season.Year}");
                var draft = await _draftRepository.Query()
                    .Where(d => d.Pick == request.Pick && d.TeamId == request.TeamId && d.Season == season.Year)
                    .FirstOrDefaultAsync();

                if (draft == null)
                {
                    Console.WriteLine($"[BACKEND] ERRO: Draft não encontrado - Pick: {request.Pick}, TeamId: {request.TeamId}, Season: {season.Year}");
                    response.Success = false;
                    response.Message = "Pick do draft não encontrado.";
                    return response;
                }
                Console.WriteLine($"[BACKEND] Draft encontrado - Pick: {draft.Pick}, TeamId: {draft.TeamId}");

                Console.WriteLine("[BACKEND] Atribuindo jogador ao draft...");
                draft.Player = player;
                draft.PlayerId = request.PlayerId;
                draft.DateTime = DateTime.Now;

                Console.WriteLine("[BACKEND] Atualizando informações do jogador...");
                player.TeamId = request.TeamId;
                player.Draft = new PlayerDraft
                {
                    Pick = request.Pick,
                    Round = request.Round,
                    Team = request.Team ?? string.Empty,
                    Year = request.Year
                };

                Console.WriteLine($"[BACKEND] Criando contrato - Pick: {request.Pick}, Season: {season.Year}");
                player.Contract = DraftUtils.RookieContracts(request.Pick, season.Year);
                player.Contract.PlayerId = player.Id; // Define o PlayerId para o contrato
                Console.WriteLine($"[BACKEND] Contrato criado - PlayerId: {player.Contract.PlayerId}, Amount: {player.Contract?.Amount}, Exp: {player.Contract?.Exp}");

                Console.WriteLine("[BACKEND] Salvando alterações no draft...");
                await _draftRepository.SaveChangesAsync();
                Console.WriteLine("[BACKEND] Draft salvo com sucesso");

                Console.WriteLine("[BACKEND] Salvando alterações no jogador...");
                await _playerRepository.SaveChangesAsync();
                Console.WriteLine("[BACKEND] Jogador salvo com sucesso");

                response.Success = true;
                response.Message = $"Jogador {player.Name} selecionado no pick {request.Pick}.";
                Console.WriteLine($"[BACKEND] SUCESSO: {response.Message}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BACKEND] EXCEÇÃO: {ex.GetType().Name} - {ex.Message}");
                Console.WriteLine($"[BACKEND] StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[BACKEND] InnerException: {ex.InnerException.Message}");
                }
                response.Success = false;
                response.Message = $"Erro ao draftar o jogador: {ex.Message}";
                return response;
            }
        }

        private const int DraftPoolTeamId = 22;
        private const int FreeAgentsTeamId = 21;

        public async Task<ServiceResponse<bool>> FinalizeDraft()
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var playersInDraftPool = await _playerRepository.Query()
                    .Where(p => p.TeamId == DraftPoolTeamId)
                    .ToListAsync();

                foreach (var player in playersInDraftPool)
                {
                    player.TeamId = FreeAgentsTeamId;
                }

                await _playerRepository.SaveChangesAsync();
                response.Success = true;
                response.Message = $"{playersInDraftPool.Count} jogador(es) do draft pool enviado(s) para free agency.";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro ao finalizar draft: {ex.Message}";
                return response;
            }
        }
    }
}
