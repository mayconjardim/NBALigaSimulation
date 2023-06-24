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

            // Gerando jogos do East
            foreach (Team homeTeam in eastConferenceTeams)
            {
                foreach (Team awayTeam in eastConferenceTeams.Where(t => t != homeTeam))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Game game = new Game
                        {
                            HomeTeam = homeTeam,
                            AwayTeam = awayTeam
                        };

                        games.Add(game);
                    }
                }
            }

            // Gerando jogos do West
            foreach (Team homeTeam in westConferenceTeams)
            {
                foreach (Team awayTeam in westConferenceTeams.Where(t => t != homeTeam))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Game game = new Game
                        {
                            HomeTeam = homeTeam,
                            AwayTeam = awayTeam
                        };

                        games.Add(game);
                    }
                }
            }

            // Gerando jogos entre conferências
            foreach (Team homeTeam in teams)
            {
                List<Team> opponents = teams.Where(t => t.Conference != homeTeam.Conference && t != homeTeam).ToList();

                foreach (Team awayTeam in opponents)
                {
                    Game game = new Game
                    {
                        HomeTeam = homeTeam,
                        AwayTeam = awayTeam
                    };

                    games.Add(game);
                }
            }

            List<Game> Week1 = new List<Game>();
            List<Game> Week2 = new List<Game>();
            List<Game> Week3 = new List<Game>();
            List<Game> Week4 = new List<Game>();
            List<Game> Week5 = new List<Game>();
            List<Game> Week6 = new List<Game>();
            List<Game> Week7 = new List<Game>();
            List<Game> Week8 = new List<Game>();
            List<Game> Week9 = new List<Game>();
            List<Game> Week10 = new List<Game>();
            List<Game> Week11 = new List<Game>();
            List<Game> Week12 = new List<Game>();
            List<Game> Week13 = new List<Game>();


            // Lista de semanas
            List<List<Game>> weeks = new List<List<Game>>
            {
                Week1, Week2, Week3, Week4, Week5, Week6, Week7, Week8, Week9, Week10, Week11, Week12, Week13
            };

            int gamesPerWeek = games.Count / weeks.Count; // Número de jogos por semana (aproximadamente)

            // Distribuir os jogos nas semanas
            int currentIndex = 0;
            foreach (var week in weeks)
            {
                for (int i = 0; i < gamesPerWeek; i++)
                {
                    if (currentIndex >= games.Count)
                        break;

                    week.Add(games[currentIndex]);
                    currentIndex++;
                }
            }

            // Se houver jogos restantes, adicionar à última semana
            while (currentIndex < games.Count)
            {
                Week13.Add(games[currentIndex]);
                currentIndex++;
            }

            // Juntar os jogos de todas as semanas em uma única lista
            List<Game> allGames = Week1.Concat(Week2)
                                      .Concat(Week3)
                                      .Concat(Week4)
                                      .Concat(Week5)
                                      .Concat(Week6)
                                      .Concat(Week7)
                                      .Concat(Week8)
                                      .Concat(Week9)
                                      .Concat(Week10)
                                      .Concat(Week11)
                                      .Concat(Week12)
                                      .Concat(Week13)
                                      .ToList();

            // Ordenar os jogos pela semana
            List<Game> sortedGames = allGames.OrderBy(game => game.Week).ToList();

            // Utilize a lista sortedGames conforme desejado
        }


    }
}
