namespace NBALigaSimulation.Client.Services.NewsService
{
    public class NewsService : INewsService
    {

        private readonly HttpClient _http;

        public NewsService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ServiceResponse<List<NewsDto>>> GetAllNews()
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<List<NewsDto>>>($"api/news");
            return response;
        }

    }
}
