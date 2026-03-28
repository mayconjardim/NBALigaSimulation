namespace NBALigaSimulation.Shared.Models.Utils
{
    public static class GlobalConstants
    {
        /// <summary>
        /// ID fixo da equipe que representa os Agentes Livres (jogadores sem time).
        /// </summary>
        public const int FreeAgentsTeamId = 21;

        /// <summary>
        /// ID fixo da equipe que representa o pool do Draft (jogadores novatos).
        /// </summary>
        public const int DraftPoolTeamId = 22;

        /// <summary>
        /// Quantidade de anos à frente para gerar picks de draft automaticamente.
        /// </summary>
        public const int DraftPicksYearsAhead = 4;

        /// <summary>
        /// Quantidade padrão de rounds em um draft.
        /// </summary>
        public const int DraftRounds = 2;
    }
}
