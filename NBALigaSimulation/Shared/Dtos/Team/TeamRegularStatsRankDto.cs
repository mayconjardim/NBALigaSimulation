using System.Globalization;

namespace NBALigaSimulation.Shared.Dtos
{
    public class TeamRegularStatsRankDto
    {

        public int Id { get; set; }
        public int TeamId { get; set; }
        public int Season { get; set; }
        public int HomeWins { get; set; }
        public int HomeLosses { get; set; }
        public int RoadWins { get; set; }
        public int RoadLosses { get; set; }
        public int Points { get; set; }
        public int AllowedPoints { get; set; }
        public int Steals { get; set; }
        public int Rebounds { get; set; }
        public int Assists { get; set; }

        public string Ppg
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double pf = (double)Points / games;
                return pf.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }


        public string Oppg
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double pa = (double)AllowedPoints / games;
                return pa.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string Apg
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double ast = (double)Assists / games;
                return ast.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string Rpg
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double reb = (double)Rebounds / games;
                return reb.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

    }
}
