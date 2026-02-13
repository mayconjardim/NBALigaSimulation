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

    public static List<string> Analyze(PlayerRatings p, string pos)
    {
        var lines = new List<string>();
        if (p == null || string.IsNullOrEmpty(pos)) return lines;

        var profileKey = NormalizePositionForProfile(pos);
        if (!ScoutingProfiles.Profiles.TryGetValue(profileKey, out var profile))
            return lines;

        foreach (var attr in profile.CoreAttributes)
        {
            int value = GetValue(p, attr);
            var tier = TierHelper.GetTier(value);

            if (tier == Tier.VeryWeak || tier == Tier.BelowAverage)
                lines.Add($"{attr} é uma limitação clara para a posição.");
        }

        foreach (var attr in profile.BonusAttributes)
        {
            int value = GetValue(p, attr);
            var tier = TierHelper.GetTier(value);

            if (tier == Tier.Good || tier == Tier.Elite)
                lines.Add($"{attr} acima do esperado traz um diferencial interessante.");
        }

        return lines;
    }
}
