using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Drafts;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.League;

public partial class Draft : ComponentBase
{
    [Inject] public IDraftService DraftService { get; set; }
    [Inject] public IPlayerService PlayerService { get; set; }
    [Inject] public ILocalStorageService LocalStorage { get; set; }

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

    private bool _draftPoolLoading = false;
    private List<PlayerCompleteDto> _draftPool = new();
    private string _draftPoolSearch = "";
    private string _modalSearch = "";

    private DraftDto? _selectedPick = null;
    private int? _userTeamId = null;

    protected override async Task OnInitializedAsync()
    {
        await LoadUserTeamId();
        await LoadData();
        _loadingInitial = false;
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

    private async Task LoadData()
    {
        try
        {
            _message = string.Empty;

            var lotteryResponse = await DraftService.GetLastLottery();
            if (lotteryResponse != null && lotteryResponse.Success)
            {
                _lottery = lotteryResponse.Data;
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

    private async Task LoadDraftPool()
    {
        if (_draftPool.Any()) return;

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
            if (string.IsNullOrWhiteSpace(_modalSearch))
                return _draftPool;

            var search = _modalSearch.ToLower();
            return _draftPool.Where(p =>
                p.Name.ToLower().Contains(search) ||
                p.Pos.ToLower().Contains(search)
            ).ToList();
        }
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
        await LoadDraftPool();
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
