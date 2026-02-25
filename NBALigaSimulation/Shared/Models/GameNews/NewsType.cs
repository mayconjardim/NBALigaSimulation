namespace NBALigaSimulation.Shared.Models.GameNews
{
    /// <summary>Tipo da notícia para filtro e link correto (jogo, jogador, time, trade).</summary>
    public enum NewsType
    {
        Game = 0,
        Trade = 1,
        FA = 2,
        Draft = 3,
        Injury = 4,
        Award = 5
    }
}
