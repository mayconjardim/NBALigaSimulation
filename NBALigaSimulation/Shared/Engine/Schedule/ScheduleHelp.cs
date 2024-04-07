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
        
        RandomUtils.Shuffle(games);
        return games;
    }
    
    public static List<Game> GenerateWeeksAndDates(List<Game> games, List<Team> Team)
    {

        List<Game> updateGames = new List<Game>();
        List<Game> homeGames = new List<Game>();
        List<Game> awayGames = new List<Game>();
        
        foreach (var team  in Team)
        {
            homeGames = games.Where(t => t.HomeTeam.Id == team.Id).ToList();

            DateTime startDate = DateTime.Today.AddDays(2);
            
            for (int i = 0; i < homeGames.Count; i++)
            {
                if (i <= 4)
                {
                    homeGames[i].Week = "1";
                    homeGames[i].GameDate = startDate.AddDays(4);

                }
                else if (i <= 8)
                {
                    homeGames[i].Week = "2";
                    homeGames[i].GameDate = startDate.AddDays(6);
                }
                else if (i <= 12)
                {
                    homeGames[i].Week = "3";
                    homeGames[i].GameDate = startDate.AddDays(8);
                }
                else if (i <= 16)
                {
                    homeGames[i].Week = "4";
                    homeGames[i].GameDate = startDate.AddDays(10);
                }
                else if (i <= 20)
                {
                    homeGames[i].Week = "5";
                    homeGames[i].GameDate = startDate.AddDays(12);
                }
                else if (i <= 24)
                {
                    homeGames[i].Week = "6";
                    homeGames[i].GameDate = startDate.AddDays(14);
                }
                else if (i <= 28)
                {
                    homeGames[i].Week = "7";
                    homeGames[i].GameDate = startDate.AddDays(16);
                }
                else if (i <= 32)
                {
                    homeGames[i].Week = "8";
                    homeGames[i].GameDate = startDate.AddDays(18);
                }
                else 
                {
                    homeGames[i].Week = "9";
                    homeGames[i].GameDate = startDate.AddDays(20);
                }
            
            }
        }

        foreach (var team in Team)
        {
            awayGames = games.Where(t => t.AwayTeam.Id == team.Id).ToList();
            DateTime startDate = DateTime.Today.AddDays(2);

            for (int i = 0; i < awayGames.Count; i++)
            {
                if (i <= 4)
                {
                    awayGames[i].Week = "1";
                    awayGames[i].GameDate = startDate.AddDays(4);

                }
                else if (i <= 8)
                {
                    awayGames[i].Week = "2";
                    awayGames[i].GameDate = startDate.AddDays(6);
                }
                else if (i <= 12)
                {
                    awayGames[i].Week = "3";
                    awayGames[i].GameDate = startDate.AddDays(8);
                }
                else if (i <= 16)
                {
                    awayGames[i].Week = "4";
                    awayGames[i].GameDate = startDate.AddDays(10);
                }
                else if (i <= 20)
                {
                    awayGames[i].Week = "5";
                    awayGames[i].GameDate = startDate.AddDays(12);
                }
                else if (i <= 24)
                {
                    awayGames[i].Week = "6";
                    awayGames[i].GameDate = startDate.AddDays(14);
                }
                else if (i <= 28)
                {
                    awayGames[i].Week = "7";
                    awayGames[i].GameDate = startDate.AddDays(16);
                }
                else if (i <= 32)
                {
                    awayGames[i].Week = "8";
                    awayGames[i].GameDate = startDate.AddDays(18);
                }
                else 
                {
                    awayGames[i].Week = "9";
                    awayGames[i].GameDate = startDate.AddDays(20);
                }
          
            }
        }

        updateGames.AddRange(homeGames);
        updateGames.AddRange(awayGames);
        return updateGames;
    }
  

}
