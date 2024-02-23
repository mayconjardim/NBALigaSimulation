using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.PlayersService;

public class PlayersService : IPlayersService
{
    
    private readonly HttpClient _http;

    public PlayersService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ServiceResponse<PlayerCompleteDto>> GetPlayerById(int playerId)
    {
        var response = new ServiceResponse<PlayerCompleteDto>();

        try
        {
            var result = await _http.GetFromJsonAsync<PlayerCompleteDto>($"api/players/{playerId}");
            response.Data = result;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Ocorreu um erro ao buscar o jogador com o ID {playerId}: {ex.Message}";
        }

        return response;
    }

}