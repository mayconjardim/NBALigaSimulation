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
            List<Team> opponents = teams.Where(t => t.Conference != homeTeam.Conference && t != homeTeam && t.IsHuman 
                == true).ToList();

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
        
        return games;
    }

    public static List<Game> UpdateDates(List<Game> games, List<Team> teams)
    {
        
        HashSet<int> addedGameIds = new HashSet<int>();
        
        //1
        List<Game> week1 = new List<Game>();
        foreach (var teamId in teams.Select(t => t.Id))
        {
            List<Game> homeGames = new List<Game>();  
            homeGames.AddRange(games.Where(g => g.HomeTeam.Id == teamId && !addedGameIds.Contains(g.Id)).Take(2));
            
            List<Game> awayGames = new List<Game>(); 
            awayGames.AddRange(games.Where(g => g.AwayTeam.Id == teamId && !addedGameIds.Contains(g.Id)).Take(2));
       
            week1.AddRange(homeGames);
            week1.AddRange(awayGames);
            
            foreach (var game in homeGames.Concat(awayGames))
            {
                addedGameIds.Add(game.Id);
                game.Week = "1";
            }
        }

        //2
        List<Game> week2 = new List<Game>();

        foreach (var teamId in teams.Select(t => t.Id))
        {
            List<Game> homeGames = new List<Game>();  
            homeGames.AddRange(games.Where(g => g.HomeTeam.Id == teamId && !addedGameIds.Contains(g.Id)).Take(2));
          
            List<Game> awayGames = new List<Game>(); 
            awayGames.AddRange(games.Where(g => g.AwayTeam.Id == teamId && !addedGameIds.Contains(g.Id)).Take(2));
            
            week2.AddRange(homeGames);
            week2.AddRange(awayGames);
            
            foreach (var game in homeGames.Concat(awayGames))
            {
                addedGameIds.Add(game.Id);
                game.Week = "2";

            }
        }

        //3
        List<Game> week3 = new List<Game>();

        foreach (var teamId in teams.Select(t => t.Id))
        {
            List<Game> homeGames = new List<Game>();  
            homeGames.AddRange(games.Where(g => g.HomeTeam.Id == teamId && !addedGameIds.Contains(g.Id)).Take(2));
         
            List<Game> awayGames = new List<Game>(); 
            awayGames.AddRange(games.Where(g => g.AwayTeam.Id == teamId && !addedGameIds.Contains(g.Id)).Take(2));
            
            week3.AddRange(homeGames);
            week3.AddRange(awayGames);
            
            foreach (var game in homeGames.Concat(awayGames))
            {
                addedGameIds.Add(game.Id);
                game.Week = "3";

            }
        }

        //4
        List<Game> week4 = new List<Game>();

        foreach (var teamId in teams.Select(t => t.Id))
        {
            List<Game> homeGames = new List<Game>();  
            homeGames.AddRange(games.Where(g => g.HomeTeam.Id == teamId && !addedGameIds.Contains(g.Id)).Take(2));
         
            List<Game> awayGames = new List<Game>(); 
            awayGames.AddRange(games.Where(g => g.AwayTeam.Id == teamId && !addedGameIds.Contains(g.Id)).Take(2));
            
            week4.AddRange(homeGames);
            week4.AddRange(awayGames);
            
            foreach (var game in homeGames.Concat(awayGames))
            {
                addedGameIds.Add(game.Id);
                game.Week = "4";

            }
        }

        //5
        List<Game> week5 = new List<Game>();

        foreach (var teamId in teams.Select(t => t.Id))
        {
            List<Game> homeGames = new List<Game>();  
            homeGames.AddRange(games.Where(g => g.HomeTeam.Id == teamId && !addedGameIds.Contains(g.Id)).Take(2));
         
            List<Game> awayGames = new List<Game>(); 
            awayGames.AddRange(games.Where(g => g.AwayTeam.Id == teamId && !addedGameIds.Contains(g.Id)).Take(2));
          
            week5.AddRange(homeGames);
            week5.AddRange(awayGames);
            
            foreach (var game in homeGames.Concat(awayGames))
            {
                addedGameIds.Add(game.Id);
                game.Week = "5";
            }
        }

        //6
        List<Game> week6 = new List<Game>();

        foreach (var teamId in teams.Select(t => t.Id))
        {
            List<Game> homeGames = new List<Game>();  
            homeGames.AddRange(games.Where(g => g.HomeTeam.Id == teamId && !addedGameIds.Contains(g.Id)).Take(2));
          
            List<Game> awayGames = new List<Game>(); 
            awayGames.AddRange(games.Where(g => g.AwayTeam.Id == teamId && !addedGameIds.Contains(g.Id)).Take(2));
           
            week6.AddRange(homeGames);
            week6.AddRange(awayGames);
            
            foreach (var game in homeGames.Concat(awayGames))
            {
                addedGameIds.Add(game.Id);
                game.Week = "6";
            }
        }

        //7
        List<Game> week7 = new List<Game>();

        foreach (var teamId in teams.Select(t => t.Id))
        {
            List<Game> homeGames = new List<Game>();  
            homeGames.AddRange(games.Where(g => g.HomeTeam.Id == teamId && !addedGameIds.Contains(g.Id)).Take(2));
          
            List<Game> awayGames = new List<Game>(); 
            awayGames.AddRange(games.Where(g => g.AwayTeam.Id == teamId && !addedGameIds.Contains(g.Id)).Take(2));
         
            week7.AddRange(homeGames);
            week7.AddRange(awayGames);
            
            foreach (var game in homeGames.Concat(awayGames))
            {
                addedGameIds.Add(game.Id);
                game.Week = "7";
            }
        }

        //8
        List<Game> week8 = new List<Game>();

        foreach (var teamId in teams.Select(t => t.Id))
        {
            List<Game> homeGames = new List<Game>();  
            homeGames.AddRange(games.Where(g => g.HomeTeam.Id == teamId && !addedGameIds.Contains(g.Id)).Take(3));
        
            List<Game> awayGames = new List<Game>(); 
            awayGames.AddRange(games.Where(g => g.AwayTeam.Id == teamId && !addedGameIds.Contains(g.Id)).Take(3));
          
            week8.AddRange(homeGames);
            week8.AddRange(awayGames);
            
            foreach (var game in homeGames.Concat(awayGames))
            {
                addedGameIds.Add(game.Id);
                game.Week = "8";
            }
        }

        //9
        List<Game> week9 = new List<Game>();

        foreach (var teamId in teams.Select(t => t.Id))
        {
            List<Game> homeGames = new List<Game>();  
            homeGames.AddRange(games.Where(g => g.HomeTeam.Id == teamId && !addedGameIds.Contains(g.Id)).Take(5));
            
            List<Game> awayGames = new List<Game>(); 
            awayGames.AddRange(games.Where(g => g.AwayTeam.Id == teamId && !addedGameIds.Contains(g.Id)).Take(5));
            
            week9.AddRange(homeGames);
            week9.AddRange(awayGames);
            
            foreach (var game in homeGames.Concat(awayGames))
            {
                addedGameIds.Add(game.Id);
                game.Week = "9";
            }
        }
        DateTime startDate = DateTime.Now;
        int daysBetweenWeeks = 2;

        List<List<Game>> weeks = new List<List<Game>> { week1, week2, week3, week4, week5, week6, week7, week8, week9 };

        for (int i = 0; i < weeks.Count; i++)
        {

            foreach (Game game in weeks[i])
            {
                game.GameDate = startDate.AddDays((i + 1) * daysBetweenWeeks);
            }
        }
              
        for (int i = 0; i < weeks.Count; i++)
        {
            Console.WriteLine($"Semana {i + 1} contem {weeks[i].Count} jogos!");
        }

        Console.WriteLine("Ids contados: " + addedGameIds.Count);

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

}
