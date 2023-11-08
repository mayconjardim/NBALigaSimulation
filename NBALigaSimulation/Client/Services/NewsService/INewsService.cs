namespace NBALigaSimulation.Client.Services.NewsService
{
    public interface INewsService
    {

        Task<ServiceResponse<List<NewsDto>>> GetAllNews();


    }
}
