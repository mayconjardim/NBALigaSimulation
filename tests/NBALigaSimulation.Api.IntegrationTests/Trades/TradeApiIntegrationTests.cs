using System.Net;
using System.Net.Http.Json;
using NBALigaSimulation.Api.IntegrationTests.Infrastructure;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Dtos.Trades;
using NBALigaSimulation.Shared.Models.Utils;
using Xunit;

namespace NBALigaSimulation.Api.IntegrationTests.Trades;

public class TradeApiIntegrationTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient _client;

    public TradeApiIntegrationTests(ApiTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTrade_ReturnsBadRequest_WhenProposalHasNoAssetsFromBothTeams()
    {
        var dto = new TradeCreateDto
        {
            TeamOneId = 1,
            TeamTwoId = 2,
            TradePlayers = new(),
            Players = new()
            {
                BuildPlayerDto(10, 1, "Guard One", "PG")
            },
            DraftPicks = new()
        };

        var response = await _client.PostAsJsonAsync("/api/trades", dto);
        var payload = await TestJson.ReadAsync<ServiceResponse<TradeDto>>(response.Content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
    }

    [Fact]
    public async Task CreateTrade_ThenGetById_ReturnsTradeWithPlayersAndPicks()
    {
        var dto = new TradeCreateDto
        {
            TeamOneId = 1,
            TeamTwoId = 2,
            TradePlayers = new(),
            Players = new()
            {
                BuildPlayerDto(10, 1, "Guard One", "PG"),
                BuildPlayerDto(11, 2, "Wing Two", "SG")
            },
            DraftPicks = new()
            {
                new() { Id = 3000, TeamId = 1, Original = "RIO", Year = 2027, Round = 1, TeamName = "RIO" }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/trades", dto);
        var createPayload = await TestJson.ReadAsync<ServiceResponse<TradeDto>>(createResponse.Content);

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        Assert.NotNull(createPayload);
        Assert.True(createPayload.Success);
        Assert.NotNull(createPayload.Data);

        var getResponse = await _client.GetAsync($"/api/trades/{createPayload.Data.Id}");
        var getPayload = await TestJson.ReadAsync<ServiceResponse<TradeDto>>(getResponse.Content);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(getPayload);
        Assert.True(getPayload.Success);
        Assert.NotNull(getPayload.Data);
        Assert.Equal(2, getPayload.Data.Players.Count);
        Assert.Single(getPayload.Data.DraftPicks);
    }

    private static Shared.Dtos.Players.PlayerCompleteDto BuildPlayerDto(int id, int teamId, string name, string pos)
    {
        return new Shared.Dtos.Players.PlayerCompleteDto
        {
            Id = id,
            TeamId = teamId,
            Name = name,
            Pos = pos,
            Born = new Born { Year = 2000, Loc = "Brazil" },
            TeamName = teamId == 1 ? "Rio Stars" : "Sampa Kings",
            TeamAbrv = teamId == 1 ? "RIO" : "SPK",
            ImgUrl = "Images/Utils/blank.png",
            Ratings = new(),
            RegularStats = new(),
            PlayoffsStats = new(),
            Stats = new(),
            PlayerAwards = new()
        };
    }
}
