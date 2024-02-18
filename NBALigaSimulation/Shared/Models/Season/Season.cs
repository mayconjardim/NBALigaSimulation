using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Models.Season
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
            foreach (int teamId in teams.Select(t => t.Id))
            {
                var homeGames = games.Where(g => g.HomeTeam.Id == teamId).Take(2);
                var awayGames = games.Where(g => g.AwayTeam.Id == teamId).Take(2);

                week1.AddRange(homeGames);
                week1.AddRange(awayGames);
            }
            RandomUtils.Shuffle(week1);

            List<Game> week2 = new List<Game>();
            foreach (int teamId in teams.Select(t => t.Id))
            {
                var homeGames = games.Where(g => g.HomeTeam.Id == teamId).Take(2);
                var awayGames = games.Where(g => g.AwayTeam.Id == teamId).Take(2);

                week2.AddRange(homeGames);
                week2.AddRange(awayGames);
            }
            RandomUtils.Shuffle(week2);

            List<Game> week3 = new List<Game>();
            foreach (int teamId in teams.Select(t => t.Id))
            {
                var homeGames = games.Where(g => g.HomeTeam.Id == teamId).Take(2);
                var awayGames = games.Where(g => g.AwayTeam.Id == teamId).Take(2);

                week3.AddRange(homeGames);
                week3.AddRange(awayGames);
            }
            RandomUtils.Shuffle(week3);

            List<Game> week4 = new List<Game>();
            foreach (int teamId in teams.Select(t => t.Id))
            {
                var homeGames = games.Where(g => g.HomeTeam.Id == teamId).Take(2);
                var awayGames = games.Where(g => g.AwayTeam.Id == teamId).Take(2);

                week4.AddRange(homeGames);
                week4.AddRange(awayGames);
            }
            RandomUtils.Shuffle(week4);


            List<Game> week5 = new List<Game>();
            foreach (int teamId in teams.Select(t => t.Id))
            {
                var homeGames = games.Where(g => g.HomeTeam.Id == teamId).Take(2);
                var awayGames = games.Where(g => g.AwayTeam.Id == teamId).Take(2);

                week5.AddRange(homeGames);
                week5.AddRange(awayGames);
            }
            RandomUtils.Shuffle(week5);

            List<Game> week6 = new List<Game>();
            foreach (int teamId in teams.Select(t => t.Id))
            {
                var homeGames = games.Where(g => g.HomeTeam.Id == teamId).Take(2);
                var awayGames = games.Where(g => g.AwayTeam.Id == teamId).Take(2);

                week6.AddRange(homeGames);
                week6.AddRange(awayGames);
            }
            RandomUtils.Shuffle(week6);

            List<Game> week7 = new List<Game>();
            foreach (int teamId in teams.Select(t => t.Id))
            {
                var homeGames = games.Where(g => g.HomeTeam.Id == teamId).Take(2);
                var awayGames = games.Where(g => g.AwayTeam.Id == teamId).Take(2);

                week7.AddRange(homeGames);
                week7.AddRange(awayGames);
            }
            RandomUtils.Shuffle(week7);

            List<Game> week8 = new List<Game>();
            foreach (int teamId in teams.Select(t => t.Id))
            {
                var homeGames = games.Where(g => g.HomeTeam.Id == teamId).Take(2);
                var awayGames = games.Where(g => g.AwayTeam.Id == teamId).Take(2);

                week8.AddRange(homeGames);
                week8.AddRange(awayGames);
            }
            RandomUtils.Shuffle(week8);

            List<Game> week9 = new List<Game>();
            foreach (int teamId in teams.Select(t => t.Id))
            {
                var homeGames = games.Where(g => g.HomeTeam.Id == teamId).Take(2);
                var awayGames = games.Where(g => g.AwayTeam.Id == teamId).Take(2);

                week9.AddRange(homeGames);
                week9.AddRange(awayGames);
            }
            RandomUtils.Shuffle(week9);

            List<Game> week10 = new List<Game>();
            week10 = games.Take(20).ToList();
            RandomUtils.Shuffle(week10);


            games.AddRange(week1);
            games.AddRange(week2);
            games.AddRange(week3);
            games.AddRange(week4);
            games.AddRange(week5);
            games.AddRange(week6);
            games.AddRange(week7);
            games.AddRange(week8);
            games.AddRange(week9);
            games.AddRange(week10);


            DateTime startDate = DateTime.Today.AddDays(1);
            int daysBetweenWeeks = 2;

            for (int weekIndex = 0; weekIndex < 12; weekIndex++)
            {
                List<Game> currentWeek = games.Skip(weekIndex * 62).Take(62).ToList();

                foreach (Game game in currentWeek)
                {
                    game.GameDate = startDate;
                }

                startDate = startDate.AddDays(daysBetweenWeeks);
            }



            Games = games;

        }

    }
}
