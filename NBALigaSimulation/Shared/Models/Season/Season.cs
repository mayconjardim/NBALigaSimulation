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

            Random random = new Random();

            List<Team> eastConferenceTeams = teams.Where(t => t.Conference == "East").ToList();
            List<Team> westConferenceTeams = teams.Where(t => t.Conference == "West").ToList();


            //Gerando jogos do East
            foreach (Team homeTeam in eastConferenceTeams)
            {
                foreach (Team awayTeam in eastConferenceTeams.Where(t => t != homeTeam))
                {
                    for (int i = 0; i < 3; i++)
                    {

                        Game game = new Game
                        {
                            HomeTeam = homeTeam,
                            AwayTeam = awayTeam,
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

                        Game game = new Game
                        {
                            HomeTeam = homeTeam,
                            AwayTeam = awayTeam,

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


                    Game game = new Game
                    {
                        HomeTeam = homeTeam,
                        AwayTeam = awayTeam,
                    };

                    games.Add(game);
                }
            }

            games = games.OrderBy(x => random.Next()).ToList();
            Games = games;

        }

    }
}
