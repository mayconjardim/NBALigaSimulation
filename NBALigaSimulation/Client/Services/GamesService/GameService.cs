using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Games;
using NBALigaSimulation.Shared.Models.Utils;
using NBALigaSimulation.Client.Utilities;

namespace NBALigaSimulation.Client.Services.GameService;

public class GameService : IGameService
{
    
    private readonly HttpClient _http;

    public GameService(HttpClient http)
    {
        _http = http;
    }
    
    public async Task<ServiceResponse<List<GameCompleteDto>>> GetGamesByTeamId(int teamId)
    {
        return await _http.SafeGetAsync<List<GameCompleteDto>>($"api/games/teams/{teamId}");
    }

    public async Task<ServiceResponse<List<GameCompleteDto>>> GetGamesBetweenTeams(int teamAId, int teamBId)
    {
        return await _http.SafeGetAsync<List<GameCompleteDto>>($"api/games/headtohead?teamA={teamAId}&teamB={teamBId}");
    }

    public async Task<ServiceResponse<List<GameCompleteDto>>> GetAllGames()
    {
        return await _http.SafeGetAsync<List<GameCompleteDto>>("api/games");
    }

    public string Message { get; set; } = "Carregando jogo...";

    public async Task<ServiceResponse<GameCompleteDto>> GetGameById(int gameId)
    {
        return await _http.SafeGetAsync<GameCompleteDto>($"api/games/{gameId}");
    }

    public async Task<GameCompleteDto> CreateGame(CreateGameDto game)
    {
        var result = await _http.SafePostAsync<CreateGameDto, GameCompleteDto>("api/games", game);
        return result.Data;
    }

    public async Task<ServiceResponse<bool>> UpdateGame(int gameId)
    {
        return await _http.SafePutAsync<bool, bool>($"api/games/update/{gameId}", true);
    }

    public async Task<ServiceResponse<bool>> SimGameByDateRegular()
    {
        return await _http.SafePutAsync<ServiceResponse<bool>, bool>("api/games/update/date/regular", new ServiceResponse<bool>());
    }

    public async Task<ServiceResponse<bool>> SimGameByDatePlayoffs()
    {
        return await _http.SafePutAsync<ServiceResponse<bool>, bool>("api/games/update/date/playoffs", new ServiceResponse<bool>());
    }

    public async Task<ServiceResponse<bool>> SimGameByRound(int roundNumber)
    {
        return await _http.SafePutAsync<ServiceResponse<bool>, bool>($"api/games/update/round/{roundNumber}", new ServiceResponse<bool>());
    }

    public async Task<ServiceResponse<bool>> SimPlayoffsByRound(int playoffRound)
    {
        return await _http.SafePutAsync<ServiceResponse<bool>, bool>($"api/games/update/playoffs/round/{playoffRound}", new ServiceResponse<bool>());
    }

    public async Task<ServiceResponse<bool>> SimAll()
    {
        return await _http.SafePutAsync<ServiceResponse<bool>, bool>("api/games/update/date/all", new ServiceResponse<bool>());
    }
}
