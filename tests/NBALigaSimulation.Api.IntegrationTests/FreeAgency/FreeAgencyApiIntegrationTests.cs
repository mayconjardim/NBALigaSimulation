using System.Net;
using System.Net.Http.Json;
using NBALigaSimulation.Api.IntegrationTests.Infrastructure;
using NBALigaSimulation.Shared.Dtos.FA;
using NBALigaSimulation.Shared.Models.Utils;
using Xunit;

namespace NBALigaSimulation.Api.IntegrationTests.FreeAgency;

public class FreeAgencyApiIntegrationTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;
    private readonly HttpClient _client;

    public FreeAgencyApiIntegrationTests(ApiTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateOffer_ReturnsBadRequest_WhenPlayerIsNotFreeAgent()
    {
        var dto = new FAOfferDto
        {
            PlayerId = 10,
            PlayerName = "Guard One",
            TeamId = 1,
            Season = 2026,
            Amount = 5_000_000,
            Years = 2
        };

        var response = await _client.PostAsJsonAsync("/api/fa", dto);
        var payload = await TestJson.ReadAsync<ServiceResponse<FAOfferDto>>(response.Content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
    }

    [Fact]
    public async Task CreateOffer_ThenGetByTeamId_ReturnsCreatedOffer()
    {
        _factory.FakeAuth.CurrentUserId = 100;

        var dto = new FAOfferDto
        {
            PlayerId = 12,
            PlayerName = "Free Agent",
            TeamId = 1,
            Season = 2026,
            Amount = 5_000_000,
            Years = 2
        };

        var createResponse = await _client.PostAsJsonAsync("/api/fa", dto);
        var createPayload = await TestJson.ReadAsync<ServiceResponse<FAOfferDto>>(createResponse.Content);

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        Assert.NotNull(createPayload);
        Assert.True(createPayload.Success);
        Assert.NotNull(createPayload.Data);
        Assert.Equal(12, createPayload.Data.PlayerId);

        var getResponse = await _client.GetAsync("/api/fa/teams");
        var getPayload = await TestJson.ReadAsync<ServiceResponse<List<FAOfferDto>>>(getResponse.Content);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(getPayload);
        Assert.True(getPayload.Success);
        Assert.Single(getPayload.Data);
        Assert.Equal(12, getPayload.Data[0].PlayerId);
    }
}
