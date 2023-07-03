using System.Globalization;
using System.Net;

namespace NBALigaSimulation.Shared.Dtos
{
    public class TeamRegularStatsDto
    {

        public int Id { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamRegion { get; set; }
        public string TeamAbrv { get; set; }
        public string TeamConference { get; set; }
        public int Season { get; set; }
        public int ConfRank { get; set; }

        public int HomeWins { get; set; }
        public int HomeLosses { get; set; }

        public int RoadWins { get; set; }
        public int RoadLosses { get; set; }

        public int PlayoffWins { get; set; }
        public int PlayoffLosses { get; set; }

        public int Points { get; set; }
        public int AllowedPoints { get; set; }

        public int Steals { get; set; }
        public int AllowedStealS { get; set; }

        public int Rebounds { get; set; }
        public int AllowedRebounds { get; set; }

        public int Assists { get; set; }
        public int AllowedAssists { get; set; }

        public int Blocks { get; set; }
        public int AllowedBlocks { get; set; }

        public int Turnovers { get; set; }
        public int AllowedTurnovers { get; set; }

        public int FGA { get; set; }
        public int FGM { get; set; }
        public int AllowedFGA { get; set; }
        public int AllowedFGM { get; set; }

        public int TPA { get; set; }
        public int TPM { get; set; }
        public int Allowed3PA { get; set; }
        public int Allowed3PM { get; set; }

        public int FTM { get; set; }
        public int FTA { get; set; }
        public int AllowedFTM { get; set; }
        public int AllowedFTA { get; set; }

        public int Wins
        {
            get
            {
                return (HomeWins + RoadWins);
            }
        }

        public int Losses
        {
            get
            {
                return (HomeLosses + RoadLosses);
            }
        }

        public string WinPct
        {
            get
            {
                int games = (HomeWins + HomeLosses + RoadWins + RoadLosses);
                double winPct = (double)(HomeWins + RoadWins) / games;
                return winPct.ToString("0.000", CultureInfo.InvariantCulture);
            }
        }

        public string PF
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double pf = (double)Points / games;
                return pf.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string PA
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double pa = (double)AllowedPoints / games;
                return pa.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string DIFF
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double diff = (double)(Points - AllowedPoints) / games;
                return diff.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

    }
}
