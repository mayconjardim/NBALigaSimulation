using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Engine.Schedule;

public static class ScheduleHelp
{

    public static List<Game> GenerateSchedule(List<Team> teams, Season season)
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
                        Type = 0,
                        Season = season
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
                        Type = 0,
                        Season = season
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
                    Type = 0,
                    Season = season
                };

                games.Add(game);
            }
        }

        RandomUtils.Shuffle(games);

        //1
        List<Game> week1 = new List<Game>();

        foreach (int teamId in teams.Select(t => t.Id))
        {
            var homeGames = games.Where(g => g.HomeTeam.Id == teamId).Take(4);
            var awayGames = games.Where(g => g.AwayTeam.Id == teamId).Take(4);
            week1.AddRange(homeGames);
            week1.AddRange(awayGames);
        }

        RandomUtils.Shuffle(week1);

        //2
        List<Game> week2 = new List<Game>();

        foreach (int teamId in teams.Select(t => t.Id))
        {
            var homeGames = games.Where(g => g.HomeTeam.Id == teamId).Take(4);
            var awayGames = games.Where(g => g.AwayTeam.Id == teamId).Take(4);
            week2.AddRange(homeGames);
            week2.AddRange(awayGames);
        }

        RandomUtils.Shuffle(week2);

        //3
        List<Game> week3 = new List<Game>();

        foreach (int teamId in teams.Select(t => t.Id))
        {
            var homeGames = games.Where(g => g.HomeTeam.Id == teamId).Take(4);
            var awayGames = games.Where(g => g.AwayTeam.Id == teamId).Take(4);
            week3.AddRange(homeGames);
            week3.AddRange(awayGames);
        }

        RandomUtils.Shuffle(week3);

        //4
        List<Game> week4 = new List<Game>();

        foreach (int teamId in teams.Select(t => t.Id))
        {
            var homeGames = games.Where(g => g.HomeTeam.Id == teamId).Take(4);
            var awayGames = games.Where(g => g.AwayTeam.Id == teamId).Take(4);
            week4.AddRange(homeGames);
            week4.AddRange(awayGames);
        }

        RandomUtils.Shuffle(week4);

        //5
        List<Game> week5 = new List<Game>();

        foreach (int teamId in teams.Select(t => t.Id))
        {
            var homeGames = games.Where(g => g.HomeTeam.Id == teamId).Take(4);
            var awayGames = games.Where(g => g.AwayTeam.Id == teamId).Take(4);
            week5.AddRange(homeGames);
            week5.AddRange(awayGames);
        }

        RandomUtils.Shuffle(week5);

        //6
        List<Game> week6 = new List<Game>();

        foreach (int teamId in teams.Select(t => t.Id))
        {
            var homeGames = games.Where(g => g.HomeTeam.Id == teamId).Take(4);
            var awayGames = games.Where(g => g.AwayTeam.Id == teamId).Take(4);
            week6.AddRange(homeGames);
            week6.AddRange(awayGames);
        }

        RandomUtils.Shuffle(week6);

        //7
        List<Game> week7 = new List<Game>();

        foreach (int teamId in teams.Select(t => t.Id))
        {
            var homeGames = games.Where(g => g.HomeTeam.Id == teamId).Take(4);
            var awayGames = games.Where(g => g.AwayTeam.Id == teamId).Take(4);
            week7.AddRange(homeGames);
            week7.AddRange(awayGames);
        }

        RandomUtils.Shuffle(week7);

        //8
        List<Game> week8 = new List<Game>();

        foreach (int teamId in teams.Select(t => t.Id))
        {
            var homeGames = games.Where(g => g.HomeTeam.Id == teamId).Take(4);
            var awayGames = games.Where(g => g.AwayTeam.Id == teamId).Take(4);
            week8.AddRange(homeGames);
            week8.AddRange(awayGames);
        }

        RandomUtils.Shuffle(week8);

        //9
        List<Game> week9 = new List<Game>();

        foreach (int teamId in teams.Select(t => t.Id))
        {
            var homeGames = games.Where(g => g.HomeTeam.Id == teamId).Take(5);
            var awayGames = games.Where(g => g.AwayTeam.Id == teamId).Take(5);
            week9.AddRange(homeGames);
            week9.AddRange(awayGames);
        }
        RandomUtils.Shuffle(week9);

        List<List<Game>> weeks = new List<List<Game>> { week1, week2, week3, week4, week5, week6, week7, week8, week9 };

        for (int i = 0; i < weeks.Count; i++)
        {
            Console.WriteLine($"A semana {i + 1} tem {weeks[i].Count}");
        }
        
        games.AddRange(week1);
        games.AddRange(week2);
        games.AddRange(week3);
        games.AddRange(week4);
        games.AddRange(week5);
        games.AddRange(week6);
        games.AddRange(week7);
        games.AddRange(week8);
        games.AddRange(week9);


        return games;
    }

    public static List<Game> GamesDates(List<Game> newSchedule)
    {
        List<Game> editedGames = new List<Game>();
        
        List<Game> week1 = newSchedule.Take(160).ToList();
        List<Game> week2 = newSchedule.Skip(160).Take(160).ToList();
        List<Game> week3 = newSchedule.Skip(320).Take(160).ToList();
        List<Game> week4 = newSchedule.Skip(480).Take(160).ToList();
        List<Game> week5 = newSchedule.Skip(640).Take(160).ToList();
        List<Game> week6 = newSchedule.Skip(800).Take(160).ToList();
        List<Game> week7 = newSchedule.Skip(960).Take(160).ToList();
        List<Game> week8 = newSchedule.Skip(1120).Take(160).ToList();
        List<Game> week9 = newSchedule.Skip(1280).Take(200).ToList();
        
        List<List<Game>> weeks = new List<List<Game>> { week1, week2, week3, week4, week5, week6, week7, week8, week9 };
        
        DateTime weekStartDate = DateTime.Today.AddDays(2); 

        for (int i = 0; i < weeks.Count; i++)
        {
            List<Game> week = weeks[i]; 
            foreach (var game in week)
            {
                game.GameDate = weekStartDate; 
            }
            
            weekStartDate = weekStartDate.AddDays(2); 
        }
     
        editedGames.AddRange(week1);
        editedGames.AddRange(week2);
        editedGames.AddRange(week3);
        editedGames.AddRange(week4); 
        editedGames.AddRange(week5);
        editedGames.AddRange(week6);
        editedGames.AddRange(week7);
        editedGames.AddRange(week8); 
        editedGames.AddRange(week9); 

        return editedGames;
    }

}

