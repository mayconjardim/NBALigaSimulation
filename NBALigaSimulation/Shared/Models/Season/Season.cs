using NBALigaSimulation.Shared.Engine;
using System;
using System.Linq;

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
            List<Game> gamesFinal = new List<Game>();


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

            Shuffle(games);
            int numRodadas = 74;
            int jogosPorTimePorRodada = 1;

            List<DateTime> datasRodadas = new List<DateTime>();
            DateTime dataInicial = DateTime.Now;

            for (int rodada = 0; rodada < numRodadas; rodada++)
            {
                datasRodadas.Add(dataInicial);
                dataInicial = dataInicial.AddDays(1);
            }

            for (int rodada = 0; rodada < numRodadas; rodada++)
            {

                foreach (Team time in teams)
                {
                    List<Game> jogosCasa = games.Where(g => g.HomeTeam == time).Take(jogosPorTimePorRodada).ToList();
                    List<Game> jogosFora = games.Where(g => g.AwayTeam == time).Take(jogosPorTimePorRodada).ToList();

                     DateTime dataRodadaAtual = datasRodadas[rodada];
                    foreach (Game jogo in jogosCasa)
                    {
                        jogo.GameDate = dataRodadaAtual;
                    }
                     foreach (Game jogo in jogosFora)
                    {
                        jogo.GameDate = dataRodadaAtual;
                    }

                    gamesFinal.AddRange(jogosCasa);
                    gamesFinal.AddRange(jogosFora);

                    games.RemoveAll(g => jogosCasa.Contains(g));
                    games.RemoveAll(g => jogosFora.Contains(g));
                }

            }

            Games = gamesFinal;
        }


        static void Shuffle<T>(List<T> list)
        {
            Random random = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }




    }
}
