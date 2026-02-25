using NBALigaSimulation.Shared.Dtos.GameNews;
using NBALigaSimulation.Shared.Models.GameNews;

namespace NBALigaSimulation.Client.Shared.Components;

public partial class News
{
    private List<NewsDto> news;
    private string message = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        message = "Carregando notícias...";
        var result = await NewsService.GetAllNews();
        if (!result.Success)
            message = result.Message;
        else
        {
            news = result.Data.OrderByDescending(n => n.Id).Take(15).ToList();
        }
    }

    private static string GetNewsTypeLabel(NewsDto n)
    {
        return ((NewsType)n.Type) switch
        {
            NewsType.Game => "Jogo",
            NewsType.Trade => "Trade",
            NewsType.FA => "Free Agency",
            NewsType.Draft => "Draft",
            NewsType.Injury => "Lesão",
            NewsType.Award => "Award",
            _ => "Notícia"
        };
    }

    private static string GetNewsTypeCssClass(NewsDto n)
    {
        return ((NewsType)n.Type) switch
        {
            NewsType.Game => "news-badge-game",
            NewsType.Trade => "news-badge-trade",
            NewsType.FA => "news-badge-fa",
            NewsType.Draft => "news-badge-draft",
            NewsType.Injury => "news-badge-injury",
            NewsType.Award => "news-badge-award",
            _ => "news-badge-default"
        };
    }

    private static string GetNewsTypeIcon(NewsDto n)
    {
        return ((NewsType)n.Type) switch
        {
            NewsType.Game => "🏀",
            NewsType.Trade => "🔄",
            NewsType.FA => "✍️",
            NewsType.Draft => "📋",
            NewsType.Injury => "🩹",
            NewsType.Award => "🏆",
            _ => "📰"
        };
    }
}