using System.Reflection;
using Newtonsoft.Json;

namespace NBALigaSimulation.Shared.Engine.Scouting;

/// <summary>Banco de frases de scouting por atributo e tier (carregado do phrasesjson embarcado).</summary>
public static class ScoutingPhrasesBank
{
    // attr -> tier name (VeryWeak, BelowAverage, ...) -> list of phrases
    private static Dictionary<string, Dictionary<string, List<string>>> _phrases;
    private static readonly object _lock = new object();

    /// <summary>Punchlines para jogadores mais jovens (evolução, potencial).</summary>
    private static readonly string[] PunchlinesYoung = new[]
    {
        "Pode evoluir com trabalho e minutos.",
        "Tem margem para crescer no próximo nível.",
        "Pode ser peça importante com o desenvolvimento certo.",
        "Disposição e evolução podem fazer a diferença.",
        "Talentos que podem se destacar com a evolução certa."
    };

    /// <summary>Punchlines para veteranos (idade alta) — sem falar em evolução/potencial.</summary>
    private static readonly string[] PunchlinesVeteran = new[]
    {
        "Experiência e leitura de jogo podem compensar.",
        "Pode contribuir com minutos e liderança no vestiário.",
        "Conhece o jogo e sabe o que fazer em momentos decisivos.",
        "Veterano que ainda pode ser útil em um papel definido.",
        "Contribuição imediata, sem expectativa de evolução."
    };

    /// <summary>Punchlines para estrelas (OVR alto) — já é craque, não fala em "evolução/desenvolvimento".</summary>
    private static readonly string[] PunchlinesStar = new[]
    {
        "Já é um dos pilares da liga.",
        "Nível de estrela, decisivo em momentos importantes.",
        "Um dos melhores jogadores da competição.",
        "Craque que faz a diferença quando mais importa.",
        "Nível acima da média da liga, peça central do time."
    };

    private static Dictionary<string, Dictionary<string, List<string>>> LoadPhrases()
    {
        if (_phrases != null) return _phrases;
        lock (_lock)
        {
            if (_phrases != null) return _phrases;

            var assembly = typeof(ScoutingPhrasesBank).Assembly;
            string name = null;
            foreach (var n in assembly.GetManifestResourceNames())
            {
                if (n.EndsWith("phrasesjson", StringComparison.OrdinalIgnoreCase))
                {
                    name = n;
                    break;
                }
            }
            if (name == null)
            {
                _phrases = new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.OrdinalIgnoreCase);
                return _phrases;
            }

            using var stream = assembly.GetManifestResourceStream(name);
            if (stream == null)
            {
                _phrases = new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.OrdinalIgnoreCase);
                return _phrases;
            }

            using var reader = new System.IO.StreamReader(stream);
            var json = reader.ReadToEnd();
            _phrases = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<string>>>>(json)
                ?? new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.OrdinalIgnoreCase);

            return _phrases;
        }
    }

    /// <summary>Retorna uma frase aleatória para o atributo e tier, ou null se não houver.</summary>
    public static string GetPhrase(string attr, Tier tier)
    {
        var bank = LoadPhrases();
        if (!bank.TryGetValue(attr, out var byTier)) return null;
        var tierName = tier.ToString();
        if (!byTier.TryGetValue(tierName, out var list) || list == null || list.Count == 0) return null;
        return list[Random.Shared.Next(list.Count)];
    }

    /// <summary>Retorna uma punchline aleatória para fechar o relatório. Considera nível (OVR) e idade.</summary>
    /// <param name="ageInSeason">Idade na temporada (opcional).</param>
    /// <param name="ovr">Overall do rating (1–100). Se &gt;= 70, usa punchlines de estrela.</param>
    /// <param name="pot">Potencial do rating (1–100). Usado junto com OVR para definir nível.</param>
    public static string GetPunchline(int? ageInSeason = null, int ovr = 0, int pot = 0)
    {
        const int StarOvrThreshold = 70;
        const int VeteranAgeThreshold = 32;

        // Estrela: OVR ou potencial bem acima da média — não fala em "evolução/desenvolvimento"
        if (ovr >= StarOvrThreshold || pot >= StarOvrThreshold)
        {
            var star = PunchlinesStar;
            return star[Random.Shared.Next(star.Length)];
        }
        // Veterano (idade alta): frases sem evolução
        if (ageInSeason.HasValue && ageInSeason.Value >= VeteranAgeThreshold)
        {
            var vet = PunchlinesVeteran;
            return vet[Random.Shared.Next(vet.Length)];
        }
        // Jovem / em desenvolvimento
        var young = PunchlinesYoung;
        return young[Random.Shared.Next(young.Length)];
    }
}
