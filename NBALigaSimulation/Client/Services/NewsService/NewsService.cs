using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.GameNews;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.NewsService;

public class NewsService : INewsService
{
    private readonly HttpClient _http;

    public NewsService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ServiceResponse<List<NewsDto>>> GetAllNews()
    {
        var response = new ServiceResponse<List<NewsDto>>();
        try
        {
            var httpResponse = await _http.GetAsync("api/news");
            if (!httpResponse.IsSuccessStatusCode)
            {
                response.Success = false;
                response.Message = "Não foi possível carregar as notícias. Verifique se as migrações do banco foram aplicadas (dotnet ef database update --project Server).";
                response.Data = new List<NewsDto>();
                return response;
            }
            var data = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<List<NewsDto>>>();
            response.Success = data?.Success ?? true;
            response.Data = data?.Data ?? new List<NewsDto>();
            response.Message = data?.Message;
        }
        catch (Exception)
        {
            response.Success = false;
            response.Message = "Erro ao carregar notícias. Tente novamente mais tarde.";
            response.Data = new List<NewsDto>();
        }
        return response;
    }
}