namespace NBALigaSimulation.Shared.Models
{
    public class Season
    {

        public int Id { get; set; }
        public int Year { get; set; }
        public List<Game> Games { get; set; }

        public List<Game> GenerateSchedule(List<Team> teams)
        {
            List<Game> games = new List<Game>();
            Random random = new Random();

            DateTime startDate = DateTime.Today.AddDays(1); // Próximo dia
            DateTime endDate = startDate.AddDays(13); // Duração de 13 dias

            DateTime currentDate = startDate;
            int gameCounter = 0;

            while (currentDate <= endDate)
            {
                // Verifica se é um dia em que devem ocorrer jogos
                if (gameCounter % 2 == 0)
                {
                    // Gera os jogos para o dia atual
                    foreach (Team team in teams)
                    {
                        List<Team> conferenceTeams = teams.Where(t => t.Conference == team.Conference && t != team).ToList();
                        List<Team> nonConferenceTeams = teams.Where(t => t.Conference != team.Conference).ToList();

                        // Gera 6 jogos contra times da mesma conferência (3 em casa, 3 fora)
                        List<Team> conferenceOpponents = conferenceTeams.OrderBy(t => random.Next(0, 100)).Take(6).ToList();
                        foreach (Team opponent in conferenceOpponents)
                        {
                            Game homeGame = new Game
                            {
                                HomeTeam = team,
                                AwayTeam = opponent,
                                GameDate = currentDate.AddHours(20) // 8 PM no horário de Brasília
                            };

                            Game awayGame = new Game
                            {
                                HomeTeam = opponent,
                                AwayTeam = team,
                                GameDate = currentDate.AddHours(20) // 8 PM no horário de Brasília
                            };

                            games.Add(homeGame);
                            games.Add(awayGame);
                        }

                        // Gera 2 jogos contra times da outra conferência (1 em casa, 1 fora)
                        List<Team> nonConferenceOpponents = nonConferenceTeams.OrderBy(t => random.Next(0, 100)).Take(2).ToList();
                        foreach (Team opponent in nonConferenceOpponents)
                        {
                            Game homeGame = new Game
                            {
                                HomeTeam = team,
                                AwayTeam = opponent,
                                GameDate = currentDate.AddHours(20) // 8 PM no horário de Brasília
                            };

                            Game awayGame = new Game
                            {
                                HomeTeam = opponent,
                                AwayTeam = team,
                                GameDate = currentDate.AddHours(20) // 8 PM no horário de Brasília
                            };

                            games.Add(homeGame);
                            games.Add(awayGame);
                        }
                    }
                }

                gameCounter++;
                currentDate = currentDate.AddDays(1); // Próximo dia
            }

            return games;
        }



    }
}
