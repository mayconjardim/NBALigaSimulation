using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;
using NBALigaSimulation.Client.Utilities;

namespace NBALigaSimulation.Client.Services.PlayersService;

public class PlayerContractService : IPlayerContractService
{
    private readonly HttpClient _http;

    public PlayerContractService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ServiceResponse<List<PlayerCompleteDto>>> GetExpiringPlayers(int season)
    {
        return await _http.SafeGetAsync<List<PlayerCompleteDto>>($"api/players/expiring?season={season}");
    }
}
