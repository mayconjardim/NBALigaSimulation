namespace NBALigaSimulation.Client.Services.StatsService
{
	public class StatsService : IStatsService
	{

		private readonly HttpClient _http;

		public StatsService(HttpClient http)
		{
			_http = http;
		}

		public async Task<ServiceResponse<List<TeamRegularStatsDto>>> GetAllTeamRegularStats()
		{
			var result = await _http.GetFromJsonAsync<ServiceResponse<List<TeamRegularStatsDto>>>($"api/stats/teams");
			return result;
		}

		public async Task<ServiceResponse<List<TeamRegularStatsRankDto>>> GetAllTeamRegularStatsRank()
		{
			var result = await _http.GetFromJsonAsync<ServiceResponse<List<TeamRegularStatsRankDto>>>($"api/stats/teams/ranks");
			return result;
		}

		public async Task<ServiceResponse<List<PlayerRegularStatsDto>>> GetAllPlayerRegularStats()
		{
			var result = await _http.GetFromJsonAsync<ServiceResponse<List<PlayerRegularStatsDto>>>($"api/stats/players");
			return result;
		}

		public async Task<ServiceResponse<List<PlayerPlayoffsStatsDto>>> GetAllPlayerPlayoffsStats()
		{
			var result = await _http.GetFromJsonAsync<ServiceResponse<List<PlayerPlayoffsStatsDto>>>($"api/stats/playoffs/players");
			return result;
		}

		public async Task<ServiceResponse<List<TeamPlayoffsStatsDto>>> GetAllTeamPlayoffsStats()
		{
			var result = await _http.GetFromJsonAsync<ServiceResponse<List<TeamPlayoffsStatsDto>>>($"api/stats/playoffs/teams");
			return result;
		}
	}
}
