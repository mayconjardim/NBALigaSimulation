using NBALigaSimulation.Shared.Dtos.GameNews;

namespace NBALigaSimulation.Client.Shared.Components;

public partial class News
{

    private List<NewsDto> news;

    private string message = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        message = "Carregando Stats...";

        var result = await NewsService.GetAllNews();
        if (!result.Success)
        {
            message = result.Message;
        }
        else
        {
            news = result.Data.ToList();
            news = news.OrderByDescending(n => n.Id).ToList();
            news = news.Take(12).ToList();
        }

    }

    
}