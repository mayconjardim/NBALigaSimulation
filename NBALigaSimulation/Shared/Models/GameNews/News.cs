namespace NBALigaSimulation.Shared.Models.GameNews
{
    public class News
    {
        public int Id { get; set; }
        public NewsType Type { get; set; } = NewsType.Game;

        public string Title { get; set; } = string.Empty;
        /// <summary>Resumo curto para cards (opcional).</summary>
        public string? Summary { get; set; }
        /// <summary>URL de imagem do card (opcional).</summary>
        public string? ImageUrl { get; set; }

        /// <summary>Para compatibilidade e notícias de jogo: placar/vencedor.</summary>
        public int? GameId { get; set; }
        public string? Winner { get; set; }

        /// <summary>Segmento do link: "game", "players", "teams", "tradeoffer".</summary>
        public string? LinkEntityType { get; set; }
        /// <summary>Id do recurso (jogo, jogador, time ou trade).</summary>
        public int? LinkEntityId { get; set; }
    }
}
