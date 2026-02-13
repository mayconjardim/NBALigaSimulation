using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Drafts;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.League;

public partial class Draft : ComponentBase
{
    [Inject] public IDraftService DraftService { get; set; }
    [Inject] public IPlayerService PlayerService { get; set; }
    [Inject] public ILocalStorageService LocalStorage { get; set; }
    [Inject] public IStatsService StatsService { get; set; }

    private bool _loadingInitial = true;
    private bool _isLoading = false;
    private bool _isSelecting = false;
    private int? _selectingPlayerId = null;
    private string _loadingAction = string.Empty;
    private string _message = string.Empty;
    private bool _isError = false;
    private string _activeTab = "draft";

    private async Task OnTabChange(string tab)
    {
        _activeTab = tab;
        if (tab == "pool")
        {
            await LoadDraftPool();
        }
        StateHasChanged();
    }

    private DraftLotteryDto? _lottery;
    private List<DraftDto> _draft = new();
    private List<LotteryTeamInfo> _lotteryTeamsInfo = new();

    private bool _draftPoolLoading = false;
    private List<PlayerCompleteDto> _draftPool = new();
    private string _draftPoolSearch = "";
    private string _modalSearch = "";
    private string _modalSortColumn = "POT";
    private bool _modalSortDescending = true;

    private DraftDto? _selectedPick = null;
    private int? _userTeamId = null;
    private bool _isAdmin = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadUserTeamId();
        await LoadIsAdmin();
        await LoadData();
        _loadingInitial = false;
    }

    private async Task LoadIsAdmin()
    {
        try
        {
            if (await LocalStorage.ContainKeyAsync("_isAdmin"))
                _isAdmin = await LocalStorage.GetItemAsync<bool>("_isAdmin");
        }
        catch { _isAdmin = false; }
    }

    private async Task LoadUserTeamId()
    {
        try
        {
            if (await LocalStorage.ContainKeyAsync("teamId"))
            {
                var teamIdStr = await LocalStorage.GetItemAsync<string>("teamId");
                if (int.TryParse(teamIdStr, out int teamId))
                {
                    _userTeamId = teamId;
                }
            }
        }
        catch
        {
            _userTeamId = null;
        }
    }

    private async Task<int> GetCurrentSeasonAsync()
    {
        try
        {
            if (await LocalStorage.ContainKeyAsync("season"))
            {
                var seasonStr = await LocalStorage.GetItemAsync<string>("season");
                if (int.TryParse(seasonStr, out int season))
                    return season;
            }
        }
        catch { }
        return DateTime.Now.Year;
    }

    private bool IsUserTurn(DraftDto pick)
    {
        if (_userTeamId == null) return false;
        if (pick.TeamId != _userTeamId) return false;
        if (!string.IsNullOrEmpty(pick.PlayerName)) return false;

        // Encontra o próximo pick disponível (sem jogador) em ordem crescente
        var nextAvailablePick = _draft
            .Where(d => string.IsNullOrEmpty(d.PlayerName))
            .OrderBy(d => d.Pick)
            .FirstOrDefault();

        // Só pode escolher se este é o próximo pick disponível E pertence ao usuário
        return nextAvailablePick != null && nextAvailablePick.Pick == pick.Pick && nextAvailablePick.TeamId == _userTeamId;
    }

    private bool IsUserPick(DraftDto pick)
    {
        if (_userTeamId == null) return false;
        return pick.TeamId == _userTeamId;
    }

    /// <summary>Pick atual = primeira vaga vazia em ordem.</summary>
    private DraftDto? GetCurrentPick()
    {
        return _draft
            .Where(d => string.IsNullOrEmpty(d.PlayerName))
            .OrderBy(d => d.Pick)
            .FirstOrDefault();
    }

    /// <summary>Mostra o botão "Escolher" somente para ADMIN na pick atual; ou para o usuário quando é a vez do time dele.</summary>
    private bool CanShowSelectButton(DraftDto pick)
    {
        if (!string.IsNullOrEmpty(pick.PlayerName)) return false;
        var current = GetCurrentPick();
        if (current == null || current.Pick != pick.Pick) return false;
        return _isAdmin || IsUserTurn(pick);
    }

    private async Task LoadData()
    {
        try
        {
            _message = string.Empty;

            var currentSeason = await GetCurrentSeasonAsync();
            var lotteryResponse = await DraftService.GetLastLottery();
            if (lotteryResponse != null && lotteryResponse.Success && lotteryResponse.Data != null)
            {
                var lottery = lotteryResponse.Data;
                if (lottery.Season == currentSeason)
                {
                    _lottery = lottery;
                    await LoadLotteryInfo();
                }
                else
                {
                    _lottery = null;
                    _lotteryTeamsInfo = new List<LotteryTeamInfo>();
                }
            }
            else
            {
                _lottery = null;
                _lotteryTeamsInfo = new List<LotteryTeamInfo>();
            }

            var draftResponse = await DraftService.GetLastDraft();
            if (draftResponse != null && draftResponse.Success && draftResponse.Data != null)
            {
                _draft = draftResponse.Data;
            }
            else
            {
                _draft = new List<DraftDto>();
            }
        }
        catch (Exception ex)
        {
            _draft = new List<DraftDto>();
            Console.WriteLine($"Erro ao carregar dados do draft: {ex.Message}");
        }
    }

    private async Task LoadLotteryInfo()
    {
        if (_lottery == null) return;

        try
        {
            var season = int.Parse(await LocalStorage.GetItemAsync<string>("season"));
            var statsResponse = await StatsService.GetAllTeamRegularStats(season, false, null);
            
            if (statsResponse?.Success == true && statsResponse.Data != null)
            {
                // Replica a lógica do backend: pega os 4 piores por ConfRank, depois ordena por WinPct
                var lotteryTeams = statsResponse.Data
                    .OrderByDescending(t => t.ConfRank) // Piores rankings primeiro
                    .Take(4)
                    .OrderByDescending(t => double.Parse(t.WinPct, System.Globalization.CultureInfo.InvariantCulture)) // Pior WinPct primeiro
                    .ToList();

                Console.WriteLine($"[Draft] Encontrados {lotteryTeams.Count} times para loteria:");
                foreach (var team in lotteryTeams)
                {
                    Console.WriteLine($"  - {team.TeamAbrv} (ID: {team.TeamId}, WinPct: {team.WinPct})");
                }

                // Chances baseadas nas bolas de loteria
                var lotteryBalls = new[] { 250, 199, 156, 119 };
                var totalBalls = lotteryBalls.Sum();

                _lotteryTeamsInfo = new List<LotteryTeamInfo>();

                // IDs e abreviações dos times da loteria para busca
                var lotteryTeamIds = new[] { _lottery.FirstTeamId, _lottery.SecondTeamId, _lottery.ThirdTeamId, _lottery.FourthTeamId };
                var lotteryTeamAbrvs = new[] { _lottery.FirstTeam, _lottery.SecondTeam, _lottery.ThirdTeam, _lottery.FourthTeam };

                Console.WriteLine($"[Draft] Buscando times da loteria:");
                Console.WriteLine($"  IDs: {string.Join(", ", lotteryTeamIds)}");
                Console.WriteLine($"  Abrvs: {string.Join(", ", lotteryTeamAbrvs)}");

                // Busca TODOS os times da loteria nas stats completas (não só os 4 piores)
                var allLotteryTeamsInStats = statsResponse.Data
                    .Where(t => lotteryTeamIds.Contains(t.TeamId) || 
                               (t.TeamAbrv != null && lotteryTeamAbrvs.Any(abr => t.TeamAbrv.Equals(abr, StringComparison.OrdinalIgnoreCase))))
                    .OrderByDescending(t => double.Parse(t.WinPct, System.Globalization.CultureInfo.InvariantCulture))
                    .ToList();

                Console.WriteLine($"[Draft] Encontrados {allLotteryTeamsInStats.Count} times da loteria nas stats:");
                foreach (var team in allLotteryTeamsInStats)
                {
                    Console.WriteLine($"  - {team.TeamAbrv} (ID: {team.TeamId}, WinPct: {team.WinPct})");
                }

                // Mapeia os times da loteria para suas informações
                var lotteryOrder = new[]
                {
                    (_lottery.FirstTeam, _lottery.FirstTeamId, 1),
                    (_lottery.SecondTeam, _lottery.SecondTeamId, 2),
                    (_lottery.ThirdTeam, _lottery.ThirdTeamId, 3),
                    (_lottery.FourthTeam, _lottery.FourthTeamId, 4)
                };

                for (int i = 0; i < lotteryOrder.Length; i++)
                {
                    var (teamAbrv, teamId, finalPosition) = lotteryOrder[i];
                    Console.WriteLine($"[Draft] Processando: {teamAbrv} (ID: {teamId}) -> Pick {finalPosition}");
                    
                    // Busca o time nas stats completas primeiro
                    var teamStat = allLotteryTeamsInStats.FirstOrDefault(t => 
                        t.TeamId == teamId || 
                        (t.TeamAbrv != null && t.TeamAbrv.Equals(teamAbrv, StringComparison.OrdinalIgnoreCase)));
                    
                    // Se não encontrou, busca nos 4 piores
                    if (teamStat == null)
                    {
                        teamStat = lotteryTeams.FirstOrDefault(t => 
                            t.TeamId == teamId || 
                            (t.TeamAbrv != null && t.TeamAbrv.Equals(teamAbrv, StringComparison.OrdinalIgnoreCase)));
                    }
                    
                    if (teamStat != null)
                    {
                        // Calcula a posição original baseada no WinPct comparado com todos os times da loteria
                        // Ordena todos os times da loteria por WinPct (pior primeiro)
                        var allLotteryOrdered = allLotteryTeamsInStats
                            .OrderByDescending(t => double.Parse(t.WinPct, System.Globalization.CultureInfo.InvariantCulture))
                            .ToList();
                        
                        var originalPosition = allLotteryOrdered.IndexOf(teamStat);
                        if (originalPosition == -1)
                        {
                            // Se não encontrou em allLotteryOrdered, tenta em lotteryTeams
                            originalPosition = lotteryTeams.IndexOf(teamStat);
                        }
                        originalPosition += 1; // Converte para 1-based (1º, 2º, 3º, 4º pior)
                        
                        // Garante que está entre 1 e 4
                        if (originalPosition < 1 || originalPosition > 4)
                        {
                            // Se não está entre 1-4, calcula baseado no WinPct
                            var teamWinPct = double.Parse(teamStat.WinPct, System.Globalization.CultureInfo.InvariantCulture);
                            var worseTeams = allLotteryOrdered.Count(t => 
                                double.Parse(t.WinPct, System.Globalization.CultureInfo.InvariantCulture) > teamWinPct);
                            originalPosition = worseTeams + 1;
                            if (originalPosition > 4) originalPosition = 4;
                            if (originalPosition < 1) originalPosition = 1;
                        }
                        
                        var chance = (lotteryBalls[originalPosition - 1] * 100.0 / totalBalls);
                        
                        // Na loteria: menor número = melhor pick
                        // Se finalPosition < originalPosition = SUBIU (melhorou)
                        // Se finalPosition > originalPosition = DESCEU (piorou)
                        var moved = originalPosition - finalPosition; // Positivo = subiu, negativo = desceu

                        Console.WriteLine($"[Draft] Encontrado: {teamAbrv} - Original: {originalPosition}º pior, Final: {finalPosition}º pick, Movido: {moved}, Chance: {chance:F1}%");

                        _lotteryTeamsInfo.Add(new LotteryTeamInfo
                        {
                            TeamAbrv = teamAbrv,
                            TeamName = teamStat.TeamName,
                            TeamId = teamId,
                            OriginalPosition = originalPosition,
                            FinalPosition = finalPosition,
                            Chance = chance,
                            Moved = moved,
                            WinPct = teamStat.WinPct
                        });
                    }
                    else
                    {
                        // Se não encontrou nas stats, adiciona mesmo assim com dados básicos
                        Console.WriteLine($"[Draft] AVISO: Time não encontrado nas stats: {teamAbrv} (ID: {teamId})");
                        _lotteryTeamsInfo.Add(new LotteryTeamInfo
                        {
                            TeamAbrv = teamAbrv,
                            TeamName = teamAbrv,
                            TeamId = teamId,
                            OriginalPosition = 0,
                            FinalPosition = finalPosition,
                            Chance = 0,
                            Moved = 0,
                            WinPct = "0.000"
                        });
                    }
                }
                
                Console.WriteLine($"[Draft] Total de times processados: {_lotteryTeamsInfo.Count}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar informações da loteria: {ex.Message}");
        }
    }

    private class LotteryTeamInfo
    {
        public string TeamAbrv { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public int OriginalPosition { get; set; }
        public int FinalPosition { get; set; }
        public double Chance { get; set; }
        public int Moved { get; set; }
        public string WinPct { get; set; } = string.Empty;
    }

    private async Task LoadDraftPool(bool forceReload = false)
    {
        if (!forceReload && _draftPool.Any()) return;

        _draftPoolLoading = true;
        StateHasChanged();
        try
        {
            var response = await PlayerService.GetAllDraftPlayers();
            if (response.Success && response.Data != null)
            {
                _draftPool = response.Data.OrderByDescending(p => p.Ratings?.FirstOrDefault()?.CalculateOvr ?? 0).ToList();
            }
        }
        catch (Exception ex)
        {
            _message = $"Erro ao carregar draft pool: {ex.Message}";
            _isError = true;
        }
        finally
        {
            _draftPoolLoading = false;
            StateHasChanged();
        }
    }

    private List<PlayerCompleteDto> _filteredDraftPool
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_draftPoolSearch))
                return _draftPool;

            var search = _draftPoolSearch.ToLower();
            return _draftPool.Where(p =>
                p.Name.ToLower().Contains(search) ||
                p.Pos.ToLower().Contains(search)
            ).ToList();
        }
    }

    private List<PlayerCompleteDto> _filteredModalPlayers
    {
        get
        {
            IEnumerable<PlayerCompleteDto> list = _draftPool;
            if (!string.IsNullOrWhiteSpace(_modalSearch))
            {
                var search = _modalSearch.ToLower();
                list = list.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(search)) ||
                    (p.Pos != null && p.Pos.ToLower().Contains(search))
                );
            }
            return ApplyModalSort(list).ToList();
        }
    }

    private IEnumerable<PlayerCompleteDto> ApplyModalSort(IEnumerable<PlayerCompleteDto> list)
    {
        return _modalSortColumn switch
        {
            "Nome" => _modalSortDescending
                ? list.OrderByDescending(p => p.Name, StringComparer.OrdinalIgnoreCase)
                : list.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase),
            "Pos" => list.OrderBy(p => p.Pos ?? "").ThenByDescending(p => GetPot(p)).ThenBy(p => p.Name),
            "OVR" => _modalSortDescending
                ? list.OrderByDescending(p => GetOvr(p)).ThenBy(p => p.Name)
                : list.OrderBy(p => GetOvr(p)).ThenBy(p => p.Name),
            "POT" => _modalSortDescending
                ? list.OrderByDescending(p => GetPot(p)).ThenBy(p => p.Name)
                : list.OrderBy(p => GetPot(p)).ThenBy(p => p.Name),
            _ => list.OrderByDescending(p => GetPot(p)).ThenBy(p => p.Name)
        };
    }

    private static int GetOvr(PlayerCompleteDto p) => p.Ratings?.FirstOrDefault()?.CalculateOvr ?? 0;
    private static int GetPot(PlayerCompleteDto p) => p.Ratings?.FirstOrDefault()?.Pot ?? 0;

    private void SetModalSort(string column)
    {
        if (_modalSortColumn == column)
            _modalSortDescending = !_modalSortDescending;
        else
        {
            _modalSortColumn = column;
            _modalSortDescending = column == "POT" || column == "OVR";
        }
        StateHasChanged();
    }

    private async Task ExecuteAsync(Func<Task> action, string actionName)
    {
        if (_isLoading) return;

        _isLoading = true;
        _loadingAction = actionName;
        _message = string.Empty;

        try
        {
            await action();
        }
        finally
        {
            _isLoading = false;
            _loadingAction = string.Empty;
            StateHasChanged();
        }
    }

    private async Task OnGenerateLottery()
    {
        await ExecuteAsync(async () =>
        {
            var response = await DraftService.GenerateLottery();
            _isError = !response.Success;
            _message = response.Message ?? (_isError ? "Erro ao gerar loteria." : "Loteria gerada com sucesso.");
            await LoadData();
        }, "lottery");
    }

    private async Task OnGenerateDraft()
    {
        await ExecuteAsync(async () =>
        {
            var response = await DraftService.GenerateDraft();
            _isError = !response.Success;
            _message = response.Message ?? (_isError ? "Erro ao gerar draft." : "Draft gerado com sucesso.");
            await LoadData();
        }, "draft");
    }

    private async Task OpenPlayerSelectModal(DraftDto pick)
    {
        _selectedPick = pick;
        _modalSearch = "";
        _modalSortColumn = "POT";
        _modalSortDescending = true;
        await LoadDraftPool(forceReload: true);
        StateHasChanged();
    }

    private void ClosePlayerSelectModal()
    {
        _selectedPick = null;
        _modalSearch = "";
        StateHasChanged();
    }

    private async Task SelectPlayer(PlayerCompleteDto player)
    {
        Console.WriteLine("[FRONTEND] SelectPlayer iniciado");
        
        if (_selectedPick == null)
        {
            Console.WriteLine("[FRONTEND] ERRO: _selectedPick é null");
            return;
        }
        
        if (player == null)
        {
            Console.WriteLine("[FRONTEND] ERRO: player é null");
            return;
        }

        Console.WriteLine($"[FRONTEND] Jogador selecionado - ID: {player.Id}, Nome: {player.Name}");
        Console.WriteLine($"[FRONTEND] Pick selecionado - Pick: {_selectedPick.Pick}, TeamId: {_selectedPick.TeamId}, TeamAbrv: {_selectedPick.TeamAbrv}");

        _isSelecting = true;
        _selectingPlayerId = player.Id;

        try
        {
            Console.WriteLine("[FRONTEND] Preparando dados da requisição...");
            var season = _draft.FirstOrDefault()?.Season ?? DateTime.Now.Year;
            Console.WriteLine($"[FRONTEND] Season: {season}");
            
            var request = new DraftPlayerDto
            {
                PlayerId = player.Id,
                TeamId = _selectedPick.TeamId,
                Team = _selectedPick.TeamAbrv ?? string.Empty,
                Year = season,
                Round = _selectedPick.Round,
                Pick = _selectedPick.Pick
            };
            Console.WriteLine($"[FRONTEND] Request criado - PlayerId: {request.PlayerId}, TeamId: {request.TeamId}, Pick: {request.Pick}");

            Console.WriteLine("[FRONTEND] Chamando DraftService.SelectDraftedPlayer...");
            var response = await DraftService.SelectDraftedPlayer(request);
            Console.WriteLine($"[FRONTEND] Resposta recebida - Success: {response?.Success}, Message: {response?.Message}");
            
            if (response == null)
            {
                Console.WriteLine("[FRONTEND] ERRO: response é null");
                _message = "Erro ao selecionar jogador: resposta inválida do servidor.";
                _isError = true;
                return;
            }
            
            if (response.Success)
            {
                Console.WriteLine("[FRONTEND] Seleção bem-sucedida, recarregando dados...");
                
                // Salva os valores antes de fechar o modal (que define _selectedPick como null)
                var selectedPickNumber = _selectedPick?.Pick ?? 0;
                var playerName = player?.Name ?? "Desconhecido";
                
                try
                {
                    await LoadData();
                    Console.WriteLine("[FRONTEND] Dados recarregados com sucesso");
                }
                catch (Exception loadEx)
                {
                    Console.WriteLine($"[FRONTEND] ERRO ao recarregar dados: {loadEx.GetType().Name} - {loadEx.Message}");
                    Console.WriteLine($"[FRONTEND] StackTrace: {loadEx.StackTrace}");
                    if (loadEx.InnerException != null)
                    {
                        Console.WriteLine($"[FRONTEND] InnerException: {loadEx.InnerException.Message}");
                    }
                }
                
                Console.WriteLine("[FRONTEND] Fechando modal...");
                ClosePlayerSelectModal();
                _message = $"Jogador {playerName} selecionado no pick {selectedPickNumber}!";
                _isError = false;
                Console.WriteLine("[FRONTEND] Processo concluído com sucesso");
            }
            else
            {
                Console.WriteLine($"[FRONTEND] ERRO na resposta: {response.Message}");
                _message = response.Message ?? "Erro ao selecionar jogador.";
                _isError = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FRONTEND] EXCEÇÃO: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"[FRONTEND] StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[FRONTEND] InnerException: {ex.InnerException.Message}");
                Console.WriteLine($"[FRONTEND] InnerException StackTrace: {ex.InnerException.StackTrace}");
            }
            _message = $"Erro ao selecionar jogador: {ex.Message}";
            _isError = true;
        }
        finally
        {
            Console.WriteLine("[FRONTEND] Finalizando SelectPlayer...");
            _isSelecting = false;
            _selectingPlayerId = null;
            StateHasChanged();
        }
    }
}
