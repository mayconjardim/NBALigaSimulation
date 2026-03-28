using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.PlayersService;

public class PlayerEvolutionService : IPlayerEvolutionService
{
    private readonly HttpClient _http;

    public PlayerEvolutionService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ServiceResponse<List<PlayerCompleteDto>>> GetInjuredPlayers()
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<PlayerCompleteDto>>>("api/players/injured");
        return result ?? new ServiceResponse<List<PlayerCompleteDto>> { Success = false, Message = "Erro ao buscar jogadores lesionados." };
    }
}
