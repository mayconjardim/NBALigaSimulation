using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Drafts;
using NBALigaSimulation.Shared.Models.Utils;

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
        try
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<List<DraftDto>>>("api/draft");
            return response ?? new ServiceResponse<List<DraftDto>> 
            { 
                Success = false, 
                Message = "Resposta inválida do servidor.",
                Data = new List<DraftDto>()
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<List<DraftDto>> 
            { 
                Success = false, 
                Message = $"Erro ao buscar draft: {ex.Message}",
                Data = new List<DraftDto>()
            };
        }
    }

    public async Task<ServiceResponse<DraftLotteryDto>> GetLastLottery()
    {
        try
        {
            // Trata 404 (sem loteria ainda) sem quebrar o app
            var httpResponse = await _http.GetAsync("api/draft/lotto");

            if (!httpResponse.IsSuccessStatusCode)
            {
                return new ServiceResponse<DraftLotteryDto>
                {
                    Success = false,
                    Message = "Ainda não existe loteria gerada para o draft atual."
                };
            }

            var response = await httpResponse.Content
                .ReadFromJsonAsync<ServiceResponse<DraftLotteryDto>>();

            return response ?? new ServiceResponse<DraftLotteryDto>
            {
                Success = false,
                Message = "Resposta inválida do servidor."
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<DraftLotteryDto>
            {
                Success = false,
                Message = $"Erro ao buscar loteria: {ex.Message}"
            };
        }
    }

    public async Task<ServiceResponse<bool>> GenerateLottery()
    {
        try
        {
            var payload = new { };
            var httpResponse = await _http.PostAsJsonAsync("api/draft/generate/lottery", payload);
            var result = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            if (result == null)
                return new ServiceResponse<bool> { Success = false, Message = "Resposta inválida do servidor." };
            if (!httpResponse.IsSuccessStatusCode)
                result.Success = false;
            return result;
        }
        catch (Exception ex)
        {
            return new ServiceResponse<bool> { Success = false, Message = $"Erro ao gerar loteria: {ex.Message}" };
        }
    }

    public async Task<ServiceResponse<bool>> GenerateDraft()
    {
        try
        {
            var payload = new { };
            var httpResponse = await _http.PostAsJsonAsync("api/draft/generate/draft", payload);
            var result = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            if (result == null)
                return new ServiceResponse<bool> { Success = false, Message = "Resposta inválida do servidor." };
            if (!httpResponse.IsSuccessStatusCode)
                result.Success = false;
            return result;
        }
        catch (Exception ex)
        {
            return new ServiceResponse<bool> { Success = false, Message = $"Erro ao gerar draft: {ex.Message}" };
        }
    }

    public async Task<ServiceResponse<bool>> SelectDraftedPlayer(DraftPlayerDto request)
    {
        try
        {
            var httpResponse = await _http.PutAsJsonAsync("api/draft/select", request);
            var result = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            
            if (result == null)
            {
                return new ServiceResponse<bool> 
                { 
                    Success = false, 
                    Message = "Resposta inválida do servidor." 
                };
            }
            
            if (!httpResponse.IsSuccessStatusCode)
            {
                result.Success = false;
            }
            
            return result;
        }
        catch (Exception ex)
        {
            return new ServiceResponse<bool> 
            { 
                Success = false, 
                Message = $"Erro ao selecionar jogador: {ex.Message}" 
            };
        }
    }

    public async Task<ServiceResponse<bool>> FinalizeDraft()
    {
        try
        {
            var payload = new { };
            var httpResponse = await _http.PostAsJsonAsync("api/draft/finalize", payload);
            var result = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            if (result == null)
                return new ServiceResponse<bool> { Success = false, Message = "Resposta inválida do servidor." };
            if (!httpResponse.IsSuccessStatusCode)
                result.Success = false;
            return result;
        }
        catch (Exception ex)
        {
            return new ServiceResponse<bool> { Success = false, Message = $"Erro ao finalizar draft: {ex.Message}" };
        }
    }
}

