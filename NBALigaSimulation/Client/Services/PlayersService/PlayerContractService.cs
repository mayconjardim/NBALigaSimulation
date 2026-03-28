using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

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
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<PlayerCompleteDto>>>($"api/players/expiring?season={season}");
        return result ?? new ServiceResponse<List<PlayerCompleteDto>> { Success = false, Message = "Erro ao buscar jogadores com contrato expirando." };
    }
}
