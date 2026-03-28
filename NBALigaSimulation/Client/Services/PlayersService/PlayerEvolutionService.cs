using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;
using NBALigaSimulation.Client.Utilities;

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
        return await _http.SafeGetAsync<List<PlayerCompleteDto>>("api/players/injured");
    }
}
