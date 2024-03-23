using NBALigaSimulation.Shared.Dtos.GameNews;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.NewsService;

public interface INewsService
{
    
    Task<ServiceResponse<List<NewsDto>>> GetAllNews();
    
}