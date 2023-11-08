namespace NBALigaSimulation.Server.Services.NewsService
{
    public interface INewsService
    {
        Task<ServiceResponse<List<NewsDto>>> GetAllNews();

    }
}
