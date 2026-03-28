using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.PlayersService;

public class PlayerImportService : IPlayerImportService
{
    private readonly HttpClient _http;

    public PlayerImportService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ServiceResponse<PlayerCompleteDto>> CreatePlayer(CreatePlayerDto request)
    {
        var response = await _http.PostAsJsonAsync("api/players", request);
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<PlayerCompleteDto>>();
        return result ?? new ServiceResponse<PlayerCompleteDto> { Success = false, Message = "Erro ao criar jogador." };
    }

    public async Task<ServiceResponse<bool>> CreatePlayers(List<CreatePlayerDto> playersDto)
    {
        var response = await _http.PostAsJsonAsync("api/players/multi", playersDto);
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        return result ?? new ServiceResponse<bool> { Success = false, Message = "Erro ao criar jogadores." };
    }

    public async Task<ServiceResponse<bool>> ImportBBGMPlayers(BBGMImportDto bbgmData)
    {
        var response = await _http.PostAsJsonAsync("api/players/import-bbgm", bbgmData);
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        return result ?? new ServiceResponse<bool> { Success = false, Message = "Erro ao importar jogadores do BBGM." };
    }
}
