using System.Net;
using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.PlayersService;

public class PlayerService : IPlayerService
{
    
    private readonly HttpClient _http;

    public PlayerService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ServiceResponse<PlayerCompleteDto>> GetPlayerById(int playerId)
    {
        var response = await _http.GetAsync($"api/players/{playerId}");

        if (response.IsSuccessStatusCode)
        {
            var player = await response.Content.ReadFromJsonAsync<PlayerCompleteDto>();
            return new ServiceResponse<PlayerCompleteDto> { Data = player, Success = true};
        }
        else
        {
            return new ServiceResponse<PlayerCompleteDto> { Success = false, Message = "Não foi possivel encontrar o jogador!"};
        }
    }


    public async Task<ServiceResponse<List<PlayerSimpleDto>>> GetPlayersSearchSuggestions(string searchText)
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<PlayerSimpleDto>>>($"api/players/searchsuggestion/{searchText}");
        return result;
    }

    public async Task<ServiceResponse<List<PlayerSimpleDto>>> GetAllSimplePlayers()
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<PlayerSimpleDto>>>($"api/players/simple");
        return result;
    }

    public async Task<ServiceResponse<bool>> UpdatePlayerPtModifier(int playerId, double newPtModifier)
    {

        var result = await _http.PutAsJsonAsync($"api/players/{playerId}/ptmodifier", newPtModifier);

        bool success = result.IsSuccessStatusCode;

        var response = new ServiceResponse<bool>
        {
            Success = success,
            Data = success,
            Message = success ? "Player updated successfully." : "Failed to update player."
        };

        return response;
    }
    
    public async Task<ServiceResponse<bool>> UpdateRosterOrder(List<PlayerCompleteDto> updatedPlayerList)
    {
        var result = await _http.PutAsJsonAsync("api/players/rosterorder", updatedPlayerList);
        bool success = result.IsSuccessStatusCode;
        var response = new ServiceResponse<bool>
        {
            Success = success,
            Data = success,
            Message = success ? "Player updated successfully." : "Failed to update game."
        };

        return response;
    }

}