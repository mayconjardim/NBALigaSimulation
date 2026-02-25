using NBALigaSimulation.Shared.Models.GameNews;

namespace NBALigaSimulation.Shared.Dtos.GameNews
{
    public class NewsDto
    {
        public int Id { get; set; }
        public int Type { get; set; } // NewsType enum
        public string Title { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string? ImageUrl { get; set; }
        public int? GameId { get; set; }
        public string? Winner { get; set; }
        public string? LinkEntityType { get; set; }
        public int? LinkEntityId { get; set; }

        /// <summary>URL para o front (ex: /game/1, /players/2, /teams/3, /tradeoffer/4).</summary>
        public string LinkUrl => BuildLinkUrl();

        private string BuildLinkUrl()
        {
            if (!string.IsNullOrEmpty(LinkEntityType) && LinkEntityId.HasValue)
                return $"/{LinkEntityType}/{LinkEntityId.Value}";
            if (GameId.HasValue && Type == (int)NewsType.Game)
                return $"/game/{GameId.Value}";
            return "#";
        }
    }
}
