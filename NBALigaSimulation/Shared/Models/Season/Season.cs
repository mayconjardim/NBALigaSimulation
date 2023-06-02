using System;

namespace NBALigaSimulation.Shared.Models
{
    public class Season
    {

        public int Id { get; set; }
        public int Year { get; set; }
        public List<Game> Games { get; set; }

        public void NewSchedule(List<Team> teams)
        {
            List<Game> games = new List<Game>();

            List<Team> eastConferenceTeams = teams.Where(t => t.Conference == "East").ToList();
            List<Team> westConferenceTeams = teams.Where(t => t.Conference == "West").ToList();

            //Gerando jogos do East
            foreach (Team homeTeam in eastConferenceTeams)
            {
                foreach (Team awayTeam in eastConferenceTeams.Where(t => t != homeTeam))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        DateTime gameDate = DateTime.Now;

                        Game game = new Game
                        {
                            HomeTeam = homeTeam,
                            AwayTeam = awayTeam,
                            GameDate = gameDate
                        };

                        games.Add(game);
                    }
                }
            }

            //Gerando jogos do West
            foreach (Team homeTeam in westConferenceTeams)
            {
                foreach (Team awayTeam in westConferenceTeams.Where(t => t != homeTeam))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        DateTime gameDate = DateTime.Now;

                        Game game = new Game
                        {
                            HomeTeam = homeTeam,
                            AwayTeam = awayTeam,
                            GameDate = gameDate
                        };

                        games.Add(game);
                    }
                }
            }

            //Geranto jogos entre conferencias.
            foreach (Team homeTeam in teams)
            {
                List<Team> opponents = teams.Where(t => t.Conference != homeTeam.Conference && t != homeTeam).ToList();

                foreach (Team awayTeam in opponents)
                {
                    DateTime gameDate = DateTime.Now;

                    Game game = new Game
                    {
                        HomeTeam = homeTeam,
                        AwayTeam = awayTeam,
                        GameDate = gameDate
                    };

                    games.Add(game);
                }
            }

            Random random = new Random();

            int totalGames = games.Count;
            int gamesPerBatch = 6; // Número de jogos por lote (3 em casa, 3 fora)
            int totalBatches = totalGames / gamesPerBatch; // Total de lotes

            List<List<Game>> gameBatches = new List<List<Game>>();

            // Dividir os jogos em lotes
            for (int i = 0; i < totalBatches; i++)
            {
                List<Game> batch = new List<Game>();

                // Adicionar 3 jogos em casa e 3 jogos fora para cada equipe
                foreach (var team in teams)
                {
                    // Filtrar os jogos que envolvem a equipe atual
                    var teamGames = games.Where(g =>
                        (g.HomeTeam == team || g.AwayTeam == team) &&
                        !batch.Any(bg => bg.HomeTeam == g.HomeTeam || bg.AwayTeam == g.AwayTeam || bg.HomeTeam == g.AwayTeam || bg.AwayTeam == g.HomeTeam)
                    ).ToList();

                    // Embaralhar a lista de jogos para distribuição aleatória
                    teamGames = teamGames.OrderBy(g => random.Next()).ToList();

                    // Adicionar 3 jogos em casa
                    var homeGames = teamGames.Where(g => g.HomeTeam == team).Take(3);
                    batch.AddRange(homeGames);

                    // Adicionar 3 jogos fora
                    var awayGames = teamGames.Where(g => g.AwayTeam == team).Take(3);
                    batch.AddRange(awayGames);
                }

                gameBatches.Add(batch);
            }

            DateTime initialDate = DateTime.Now.AddDays(1); // Data inicial

            DateTime currentDate = initialDate; // Variável de referência para a data

            foreach (var batch in gameBatches)
            {
                // Atribuir a mesma data a todos os jogos do lote
                foreach (var game in batch)
                {
                    game.GameDate = currentDate;
                }

                currentDate = currentDate.AddDays(2); // Incrementar a data para o próximo lote
            }

            // Juntar todos os lotes em uma única lista
            List<Game> allGames = gameBatches.SelectMany(batch => batch).ToList();

            // 'allGames' contém todos os jogos em uma única lista, com datas atribuídas a cada lote
            Games = allGames;

        }

    }
}
