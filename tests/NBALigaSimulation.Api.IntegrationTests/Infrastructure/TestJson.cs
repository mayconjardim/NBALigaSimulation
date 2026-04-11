using System.Net.Http.Json;
using System.Text.Json;

namespace NBALigaSimulation.Api.IntegrationTests.Infrastructure;

public static class TestJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static Task<T?> ReadAsync<T>(HttpContent content)
        => content.ReadFromJsonAsync<T>(Options);
}
