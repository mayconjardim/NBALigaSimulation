using NBALigaSimulation.Shared.Models.Players;

namespace NBALigaSimulation.Shared.Engine.Scouting;

public static class ScoutingReportGenerator
{
    /// <summary>
    /// Gera o texto de scouting para um jogador a partir dos ratings e da posição.
    /// Inclui uma frase por atributo do perfil (core + bonus), ordenadas: pontos fortes → neutros → limitações, e encerra com punchline.
    /// A punchline considera: nível (OVR/Pot &gt;= 70 = estrela), idade (32+ = veterano) ou evolução para os demais.
    /// </summary>
    /// <param name="birthYear">Ano de nascimento do jogador (opcional). Usado para punchline de veterano.</param>
    public static string Generate(PlayerRatings rating, string pos, int? birthYear = null)
    {
        if (rating == null) return string.Empty;
        if (string.IsNullOrWhiteSpace(pos)) pos = "SF";

        var full = ScoutingAnalyzer.GetFullProfileAnalysis(rating, pos);
        var sentences = new List<string>();

        foreach (var item in ScoutingAnalyzer.OrderForReport(full))
        {
            var phrase = ScoutingPhrasesBank.GetPhrase(item.Attr, item.Tier);
            if (!string.IsNullOrWhiteSpace(phrase))
                sentences.Add(phrase.Trim());
        }

        if (sentences.Count == 0)
            return "Nenhuma observação de scouting gerada para esta temporada.";

        int? ageInSeason = birthYear.HasValue ? rating.Season - birthYear.Value : null;
        int ovr = rating.CalculateOvr;
        int pot = rating.Pot;
        var report = string.Join(" ", sentences);
        report += " " + ScoutingPhrasesBank.GetPunchline(ageInSeason, ovr, pot);
        return report.Trim();
    }
}
