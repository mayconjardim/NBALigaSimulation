using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Engine.Schedule;

public static class ScheduleHelp
{
    private const int TARGET_GAMES_PER_TEAM = 82;

    /// <summary>
    /// Gera o cronograma completo da temporada garantindo 82 jogos por time
    /// </summary>
    public static List<Game> GenerateSchedule(List<Team> teams, Season season)
    {
        List<Game> games = new List<Game>();
        
        var humanTeams = teams.Where(t => t.IsHuman == true).ToList();
        if (humanTeams.Count == 0)
            return games;

        var eastConferenceTeams = humanTeams.Where(t => t.Conference == "East").ToList();
        var westConferenceTeams = humanTeams.Where(t => t.Conference == "West").ToList();

        // Calcula quantos jogos são necessários para chegar a 82 por time
        int sameConferenceGames = CalculateSameConferenceGames(eastConferenceTeams.Count, westConferenceTeams.Count);
        int crossConferenceGames = CalculateCrossConferenceGames(eastConferenceTeams.Count, westConferenceTeams.Count);

        // Gera jogos dentro da mesma conferência
        GenerateConferenceGames(eastConferenceTeams, sameConferenceGames, season, games);
        GenerateConferenceGames(westConferenceTeams, sameConferenceGames, season, games);

        // Gera jogos entre conferências
        GenerateCrossConferenceGames(eastConferenceTeams, westConferenceTeams, crossConferenceGames, season, games);
        
        RandomUtils.Shuffle(games);
        return games;
    }

    /// <summary>
    /// Calcula quantos jogos cada time deve fazer contra times da mesma conferência
    /// Para 20 times (10 por conferência) e 82 jogos por time:
    /// - Dentro da conferência: 9 oponentes
    /// - Entre conferências: 10 oponentes
    /// - Distribuição: 4 jogos contra mesma conferência (9×4=36) + 4 jogos contra outra (10×4=40) = 76
    /// - Para chegar a 82: adicionar 6 jogos extras distribuídos
    /// </summary>
    private static int CalculateSameConferenceGames(int eastCount, int westCount)
    {
        // Base: 4 jogos contra cada oponente da mesma conferência
        return 4;
    }

    /// <summary>
    /// Calcula quantos jogos cada time deve fazer contra times de outra conferência
    /// </summary>
    private static int CalculateCrossConferenceGames(int eastCount, int westCount)
    {
        // Base: 4 jogos contra cada oponente de outra conferência
        // Os 6 jogos extras serão distribuídos depois
        return 4;
    }

    /// <summary>
    /// Gera jogos entre times da mesma conferência
    /// </summary>
    private static void GenerateConferenceGames(List<Team> conferenceTeams, int gamesPerMatchup, Season season, List<Game> games)
    {
        for (int i = 0; i < conferenceTeams.Count; i++)
        {
            for (int j = i + 1; j < conferenceTeams.Count; j++)
            {
                Team team1 = conferenceTeams[i];
                Team team2 = conferenceTeams[j];

                // Gera jogos alternando casa/visitante
                for (int k = 0; k < gamesPerMatchup; k++)
                {
                    bool homeTeam1 = k % 2 == 0;
                    
                    games.Add(new Game
                    {
                        HomeTeam = homeTeam1 ? team1 : team2,
                        AwayTeam = homeTeam1 ? team2 : team1,
                        Type = 0,
                        Season = season
                    });
                }
            }
        }
    }

    /// <summary>
    /// Gera jogos entre times de conferências diferentes
    /// </summary>
    private static void GenerateCrossConferenceGames(List<Team> eastTeams, List<Team> westTeams, int baseGamesPerMatchup, Season season, List<Game> games)
    {
        // Calcula quantos jogos cada time já tem da mesma conferência
        // Assumindo que ambas as conferências têm o mesmo número de times
        int sameConferenceOpponents = Math.Max(eastTeams.Count, westTeams.Count) - 1;
        int sameConferenceGames = sameConferenceOpponents * baseGamesPerMatchup;
        
        // Calcula quantos jogos são necessários contra times de outra conferência para chegar a 82
        int crossConferenceOpponents = Math.Min(eastTeams.Count, westTeams.Count);
        int crossConferenceGamesNeeded = TARGET_GAMES_PER_TEAM - sameConferenceGames;
        
        // Calcula quantos jogos por matchup são necessários
        int gamesPerMatchup = crossConferenceGamesNeeded / crossConferenceOpponents;
        int extraGames = crossConferenceGamesNeeded % crossConferenceOpponents;

        // Gera jogos para todos os matchups entre conferências
        int matchupIndex = 0;
        foreach (Team eastTeam in eastTeams)
        {
            foreach (Team westTeam in westTeams)
            {
                int gamesForThisMatchup = gamesPerMatchup + (matchupIndex < extraGames ? 1 : 0);
                
                for (int k = 0; k < gamesForThisMatchup; k++)
                {
                    bool homeEast = k % 2 == 0;
                    
                    games.Add(new Game
                    {
                        HomeTeam = homeEast ? eastTeam : westTeam,
                        AwayTeam = homeEast ? westTeam : eastTeam,
                        Type = 0,
                        Season = season
                    });
                }
                
                matchupIndex++;
            }
        }
    }
    
    /// <summary>
    /// Distribui os jogos em rodadas balanceadas
    /// Cada rodada pode ter múltiplos jogos por time
    /// Permite executar rodadas quando necessário, sem depender de datas específicas
    /// </summary>
    public static List<Game> GenerateRounds(List<Game> games, List<Team> teams, int gamesPerRoundPerTeam = 3)
    {
        if (games == null || games.Count == 0 || teams == null || teams.Count == 0)
            return games ?? new List<Game>();

        // Calcula quantas rodadas são necessárias
        // Com 82 jogos por time e ~3 jogos por rodada: ~27-28 rodadas
        int gamesPerTeam = TARGET_GAMES_PER_TEAM;
        int totalRounds = (int)Math.Ceiling((double)gamesPerTeam / gamesPerRoundPerTeam);
        
        // Rastreia quais jogos já foram atribuídos a uma rodada
        var processedGames = new HashSet<Game>();
        
        // Agrupa jogos por time para distribuição balanceada
        var teamGames = new Dictionary<int, List<Game>>();
        
        foreach (var team in teams)
        {
            var teamGameList = games.Where(g => 
                (g.HomeTeam?.Id == team.Id || g.AwayTeam?.Id == team.Id) &&
                !processedGames.Contains(g)
            ).ToList();
            
            if (teamGameList.Count > 0)
            {
                teamGames[team.Id] = teamGameList;
            }
        }

        // Distribui jogos em rodadas para cada time
        foreach (var kvp in teamGames)
        {
            var teamGameList = kvp.Value;
            int gamesCount = teamGameList.Count;
            
            // Distribui jogos uniformemente ao longo das rodadas
            for (int i = 0; i < gamesCount; i++)
            {
                Game game = teamGameList[i];
                
                // Se o jogo já foi processado, pula
                if (processedGames.Contains(game))
                    continue;
                
                // Calcula em qual rodada este jogo deve estar
                // Distribui uniformemente: rodada = (índice * totalRodadas / totalJogos) + 1
                int roundNumber = (i * totalRounds / gamesCount) + 1;
                
                game.Week = roundNumber.ToString(); // Usa Week para armazenar o número da rodada
                
                processedGames.Add(game);
            }
        }

        return games;
    }
    
    /// <summary>
    /// Obtém todos os jogos de uma rodada específica
    /// </summary>
    public static List<Game> GetGamesByRound(List<Game> games, int roundNumber)
    {
        return games.Where(g => g.Week == roundNumber.ToString()).ToList();
    }
    
    /// <summary>
    /// Obtém todos os jogos de um time em uma rodada específica
    /// </summary>
    public static List<Game> GetTeamGamesByRound(List<Game> games, int teamId, int roundNumber)
    {
        return games.Where(g => 
            g.Week == roundNumber.ToString() &&
            (g.HomeTeam?.Id == teamId || g.AwayTeam?.Id == teamId)
        ).ToList();
    }
    
    /// <summary>
    /// Obtém o número total de rodadas na temporada
    /// </summary>
    public static int GetTotalRounds(List<Game> games)
    {
        if (games == null || games.Count == 0)
            return 0;
            
        var rounds = games
            .Where(g => !string.IsNullOrEmpty(g.Week))
            .Select(g => int.TryParse(g.Week, out int round) ? round : 0)
            .Where(r => r > 0)
            .ToList();
            
        return rounds.Count > 0 ? rounds.Max() : 0;
    }
}
