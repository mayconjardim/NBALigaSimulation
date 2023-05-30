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

            Games = games;
        }




    }
}
