using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Drafts;
using NBALigaSimulation.Shared.Models.Utils;
using NBALigaSimulation.Client.Utilities;

namespace NBALigaSimulation.Client.Services.DraftService;

public class DraftService : IDraftService
{
    private readonly HttpClient _http;

    public DraftService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ServiceResponse<List<DraftDto>>> GetLastDraft()
    {
        return await _http.SafeGetAsync<List<DraftDto>>("api/draft");
    }

    public async Task<ServiceResponse<DraftLotteryDto>> GetLastLottery()
    {
        return await _http.SafeGetAsync<DraftLotteryDto>("api/draft/lotto");
    }

    public async Task<ServiceResponse<bool>> GenerateLottery()
    {
        return await _http.SafePostAsync<object, bool>("api/draft/generate/lottery", new { });
    }

    public async Task<ServiceResponse<bool>> GenerateDraft()
    {
        return await _http.SafePostAsync<object, bool>("api/draft/generate/draft", new { });
    }

    public async Task<ServiceResponse<bool>> SelectDraftedPlayer(DraftPlayerDto request)
    {
        return await _http.SafePutAsync<DraftPlayerDto, bool>("api/draft/select", request);
    }

    public async Task<ServiceResponse<bool>> FinalizeDraft()
    {
        return await _http.SafePostAsync<object, bool>("api/draft/finalize", new { });
    }
}

