using NBALigaSimulation.Shared.Models.Players;

namespace NBALigaSimulation.Shared.Engine.Scouting;

public static class ScoutingAnalyzer
{
    public static int GetValue(PlayerRatings p, string attr)
    {
        if (p == null) return 0;
        return attr switch
        {
            "Pss" => p.Pss,
            "Drb" => p.Drb,
            "Oiq" => p.Oiq,
            "Spd" => p.Spd,
            "Ins" => p.Ins,
            "Reb" => p.Reb,
            "Stre" => p.Stre,
            "Hgt" => p.Hgt,
            "Tp" => p.Tp,
            "Diq" => p.Diq,
            "Mid" => p.Fg,
            "Dnk" => p.Dnk,
            "Endu" => p.Endu,
            "Ft" => p.Ft,
            "Jmp" => p.Jmp,
            "Pot" => p.Pot,
            _ => 0
        };
    }

    /// <summary>
    /// Converte posições compostas/genéricas para o perfil base usado na análise.
    /// GF → SG; FC → C; G (Guard) → SG; F (Forward) → SF.
    /// </summary>
    public static string NormalizePositionForProfile(string pos)
    {
        if (string.IsNullOrEmpty(pos)) return pos;
        return pos.ToUpperInvariant() switch
        {
            "GF" => "SG",
            "FC" => "C",
            "G" => "SG",
            "F" => "SF",
            _ => pos
        };
    }

    public record AnalysisItem(string Attr, Tier Tier);

    public static (List<AnalysisItem> Limitations, List<AnalysisItem> Strengths) GetAnalysisData(PlayerRatings p, string pos)
    {
        var limitations = new List<AnalysisItem>();
        var strengths = new List<AnalysisItem>();
        if (p == null || string.IsNullOrEmpty(pos)) return (limitations, strengths);

        var profileKey = NormalizePositionForProfile(pos);
        if (!ScoutingProfiles.Profiles.TryGetValue(profileKey, out var profile))
            return (limitations, strengths);

        foreach (var attr in profile.CoreAttributes)
        {
            int value = GetValue(p, attr);
            var tier = TierHelper.GetTier(value);
            if (tier == Tier.VeryWeak || tier == Tier.BelowAverage)
                limitations.Add(new AnalysisItem(attr, tier));
        }

        foreach (var attr in profile.BonusAttributes)
        {
            int value = GetValue(p, attr);
            var tier = TierHelper.GetTier(value);
            if (tier == Tier.Good || tier == Tier.Elite)
                strengths.Add(new AnalysisItem(attr, tier));
        }

        return (limitations, strengths);
    }

    /// <summary>Retorna todos os atributos do perfil (core + bonus) com o tier de cada um, para relatório completo com várias frases.</summary>
    public static List<AnalysisItem> GetFullProfileAnalysis(PlayerRatings p, string pos)
    {
        var list = new List<AnalysisItem>();
        if (p == null || string.IsNullOrEmpty(pos)) return list;

        var profileKey = NormalizePositionForProfile(pos);
        if (!ScoutingProfiles.Profiles.TryGetValue(profileKey, out var profile))
            return list;

        foreach (var attr in profile.CoreAttributes)
        {
            var tier = TierHelper.GetTier(GetValue(p, attr));
            list.Add(new AnalysisItem(attr, tier));
        }
        foreach (var attr in profile.BonusAttributes)
        {
            var tier = TierHelper.GetTier(GetValue(p, attr));
            list.Add(new AnalysisItem(attr, tier));
        }
        return list;
    }

    /// <summary>Ordena para relatório: pontos fortes (Elite/Good) primeiro, depois Average, depois limitações (BelowAverage/VeryWeak).</summary>
    public static IEnumerable<AnalysisItem> OrderForReport(List<AnalysisItem> items)
    {
        if (items == null) yield break;
        int OrderKey(AnalysisItem x)
        {
            return x.Tier switch
            {
                Tier.Elite => 0,
                Tier.Good => 1,
                Tier.Average => 2,
                Tier.BelowAverage => 3,
                Tier.VeryWeak => 4,
                _ => 5
            };
        }
        foreach (var item in items.OrderBy(OrderKey))
            yield return item;
    }

    /// <summary>Retorna lista de frases usando o banco phrasesjson (uma frase por atributo do perfil, ordenadas: fortes → neutros → limitações).</summary>
    public static List<string> Analyze(PlayerRatings p, string pos)
    {
        var lines = new List<string>();
        if (p == null || string.IsNullOrEmpty(pos)) return lines;

        var full = GetFullProfileAnalysis(p, pos);
        foreach (var item in OrderForReport(full))
        {
            var phrase = ScoutingPhrasesBank.GetPhrase(item.Attr, item.Tier);
            if (!string.IsNullOrWhiteSpace(phrase))
                lines.Add(phrase.Trim());
        }
        return lines;
    }
}
