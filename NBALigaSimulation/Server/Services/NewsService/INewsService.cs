using NBALigaSimulation.Shared.Dtos.GameNews;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.NewsService
{
    public interface INewsService
    {
        
        Task<ServiceResponse<List<NewsDto>>> GetAllNews();

    }
}
