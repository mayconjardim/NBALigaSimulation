using System.Globalization;
using System.Net;
using System.Runtime.Intrinsics.X86;
using static System.Net.WebRequestMethods;

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
                return winPct.ToString(".000", CultureInfo.InvariantCulture);
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

        public string PtsPG
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double diff = (double)Points / games;
                return diff.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string AllowedPtsPG
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double diff = (double)AllowedPoints / games;
                return diff.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string RebPg
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double diff = (double)Rebounds / games;
                return diff.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string AstPg
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double diff = (double)Assists / games;
                return diff.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string BlkPg
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double diff = (double)Blocks / games;
                return diff.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string StlPg
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double diff = (double)Steals / games;
                return diff.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string TOPg
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double diff = (double)Turnovers / games;
                return diff.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string FgPg
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double diff = (double)FGM / games;
                return diff.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }
        public string FgaPg
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double diff = (double)FGA / games;
                return diff.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string FgPct
        {
            get
            {
                if (FGA == 0)
                {
                    return "0.0";
                }

                double percentage = (double)FGM / FGA * 100;
                return percentage.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string TPPg
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double diff = (double)TPM / games;
                return diff.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }
        public string TPaPg
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double diff = (double)TPA / games;
                return diff.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string TPPct
        {
            get
            {
                if (TPA == 0)
                {
                    return "0.0";
                }

                double percentage = (double)TPM / TPA * 100;
                return percentage.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string FTPg
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double diff = (double)FTM / games;
                return diff.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }
        public string FTaPg
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double diff = (double)FTA / games;
                return diff.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string FTPct
        {
            get
            {
                if (FTA == 0)
                {
                    return "0.0";
                }

                double percentage = (double)FTM / FTA * 100;
                return percentage.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string OffensiveEfficiency
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double per = (1.0 / games) * ((Points * 85.910) + (Rebounds * 53.840) + (Assists * 34.677) + (Steals * 53.840) + (Blocks * 53.840) - (FGA * 39.190) - (FTA * 20.091) - (Turnovers * 53.840));
                double stat = per / 100;
                return stat.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string DefensiveEfficiency
        {
            get
            {
                int games = HomeWins + HomeLosses + RoadWins + RoadLosses;
                double per = (1.0 / games) * ((AllowedPoints * 85.910) + (AllowedRebounds * 53.840) + (AllowedAssists * 34.677) + (AllowedStealS * 53.840) + (AllowedBlocks * 53.840) - (AllowedFGA * 39.190) - (AllowedFTA * 20.091) - (AllowedTurnovers * 53.840));
                double stat = per / 100;
                return stat.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

    }
}
