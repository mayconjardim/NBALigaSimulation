using NBALigaSimulation.Shared.Engine;

namespace NBALigaSimulation.Shared.Models
{
    public class Season
    {

        public int Id { get; set; }
        public int Year { get; set; }
        public List<Game> Games { get; set; }
        public bool IsCompleted { get; set; } = false;
        public bool LotteryCompleted { get; set; } = false;
        public bool DraftCompleted { get; set; } = false;
        public bool TcCompleted { get; set; } = false;
        public bool DeadlineCompleted { get; set; } = false;
        public bool RegularCompleted { get; set; } = false;
        public bool FirstRoundCompleted { get; set; } = false;
        public bool SecondRoundCompleted { get; set; } = false;
        public bool ThirdRoundCompleted { get; set; } = false;
        public bool FourthRoundCompleted { get; set; } = false;

        public void NewSchedule(List<Team> teams)
        {
            List<Game> games = new List<Game>();


            List<Team> eastConferenceTeams = teams.Where(t => t.Conference == "East" && t.IsHuman == true).ToList();
            List<Team> westConferenceTeams = teams.Where(t => t.Conference == "West" && t.IsHuman == true).ToList();

            // Gerando jogos do East
            foreach (Team homeTeam in eastConferenceTeams)
            {
                foreach (Team awayTeam in eastConferenceTeams.Where(t => t != homeTeam && t.IsHuman == true))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Game game = new Game
                        {
                            HomeTeam = homeTeam,
                            AwayTeam = awayTeam,
                            Type = 0
                        };

                        games.Add(game);
                    }
                }
            }

            // Gerando jogos do West
            foreach (Team homeTeam in westConferenceTeams)
            {
                foreach (Team awayTeam in westConferenceTeams.Where(t => t != homeTeam && t.IsHuman == true))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Game game = new Game
                        {
                            HomeTeam = homeTeam,
                            AwayTeam = awayTeam,
                            Type = 0
                        };

                        games.Add(game);
                    }
                }
            }

            // Gerando jogos entre conferências
            foreach (Team homeTeam in teams)
            {
                List<Team> opponents = teams.Where(t => t.Conference != homeTeam.Conference && t != homeTeam && t.IsHuman == true).ToList();

                foreach (Team awayTeam in opponents)
                {
                    Game game = new Game
                    {
                        HomeTeam = homeTeam,
                        AwayTeam = awayTeam,
                        Type = 0
                    };

                    games.Add(game);
                }
            }

            RandomUtils.Shuffle(games);

            List<Game> week1 = new List<Game>();
            List<Game> week2 = new List<Game>();
            List<Game> week3 = new List<Game>();
            List<Game> week4 = new List<Game>();
            List<Game> week5 = new List<Game>();
            List<Game> week6 = new List<Game>();
            List<Game> week7 = new List<Game>();
            List<Game> week8 = new List<Game>();
            List<Game> week9 = new List<Game>();
            List<Game> week10 = new List<Game>();
            List<Game> week11 = new List<Game>();
            List<Game> week12 = new List<Game>();


            ///week1 = games.Take(6).ToList();
            //week1 = games.Take(6).ToList();






            Games = games;
        }

    }
}
