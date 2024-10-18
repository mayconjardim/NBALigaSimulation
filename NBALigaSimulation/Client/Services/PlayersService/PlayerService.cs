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
        var response = await _http.GetFromJsonAsync<ServiceResponse<PlayerCompleteDto>>($"api/players/{playerId}");
        if (response.Success == false)
        {
            return new ServiceResponse<PlayerCompleteDto>() { Message = "Jogador não encontrado!" };
        }
        
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
    
    public async Task<ServiceResponse<PageableResponse<PlayerCompleteDto>>> GetAllFaPlayers(int currentPage, int pageSize, int season, bool isAscending, string sortedColumn,
        string position = null){
        
        var url = $"api/players/FAPlayers?page={currentPage}&pageSize={pageSize}&season={season}&isAscending={isAscending}&sortedColumn={sortedColumn}";

        if (!string.IsNullOrEmpty(position))
        {
            url += $"&position={Uri.EscapeDataString(position)}";
        }  

        var result = await _http.GetFromJsonAsync<ServiceResponse<PageableResponse<PlayerCompleteDto>>>(url);
        return result;
    }
    
    public async Task<ServiceResponse<PlayerCompleteDto>> EditPlayer(CreatePlayerDto playerDto)
    {
        var response = await _http.PostAsJsonAsync("api/players/edit-player", playerDto);

        if (!response.IsSuccessStatusCode)
        {
            return new ServiceResponse<PlayerCompleteDto>
            {
                Success = false,
                Message = "Erro ao editar o jogador."
            };
        }

        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<PlayerCompleteDto>>();
        return result ?? new ServiceResponse<PlayerCompleteDto> { Success = false, Message = "Erro ao receber a resposta do servidor." };
    }
    
}