using System.Globalization;

namespace NBALigaSimulation.Shared.Dtos.Players
{
    public class PlayerRegularStatsDto
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Pos { get; set; } = string.Empty;
        public int PlayerId { get; set; }
        public int TeamId { get; set; }
        public string TeamAbrv { get; set; }
        public int Season { get; set; }
        public int Games { get; set; }
        public int Gs { get; set; }
        public double Min { get; set; }
        public int Fg { get; set; }
        public int Fga { get; set; }
        public int FgAtRim { get; set; }
        public int FgaAtRim { get; set; }
        public int FgLowPost { get; set; }
        public int FgaLowPost { get; set; }
        public int FgMidRange { get; set; }
        public int FgaMidRange { get; set; }
        public int Tp { get; set; }
        public int Tpa { get; set; }
        public int Ft { get; set; }
        public int Fta { get; set; }
        public int Orb { get; set; }
        public int Drb { get; set; }
        public int Ast { get; set; }
        public int Tov { get; set; }
        public int Stl { get; set; }
        public int Blk { get; set; }
        public int Pf { get; set; }
        public int Pts { get; set; }
        public int Trb { get; set; }

        public string MinPG
        {
            get
            {
                double min = (double)Min / Games;
                return min.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        // FG
        public string FgPG
        {
            get
            {
                double fg = (double)Fg / Games;
                return fg.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string FgaPG
        {
            get
            {
                double fga = (double)Fga / Games;
                return fga.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string FgPct
        {
            get
            {
                if (Fga == 0)
                {
                    return "0.0";
                }

                double percentage = (double)Fg / Fga * 100;
                return percentage.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        // 3PT
        public string TpPG
        {
            get
            {
                double tpPg = (double)Tp / Games;
                return tpPg.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string TpaPG
        {
            get
            {
                double tpaPg = (double)Tpa / Games;
                return tpaPg.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string TpPct
        {
            get
            {
                if (Tpa == 0)
                {
                    return "0.0";
                }

                double percentage = (double)Tp / Tpa * 100;
                return percentage.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        // FT
        public string FtPG
        {
            get
            {
                double ftPg = (double)Ft / Games;
                return ftPg.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string FtaPG
        {
            get
            {
                double FtaPg = (double)Fta / Games;
                return FtaPg.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string FtPct
        {
            get
            {
                if (Fta == 0)
                {
                    return "0.0";
                }

                double percentage = (double)Ft / Fta * 100;
                return percentage.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        // Reb
        public string ORebPG
        {
            get
            {
                double OrebPg = (double)Orb / Games;
                return OrebPg.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string DRebPG
        {
            get
            {
                double DrebPg = (double)Drb / Games;
                return DrebPg.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string TRebPG
        {
            get
            {
                double TRebpg = (double)Trb / Games;
                return TRebpg.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        // Ast
        public string AstPG
        {
            get
            {
                double astPg = (double)Ast / Games;
                return astPg.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        // TO
        public string TovPG
        {
            get
            {
                double tovPg = (double)Tov / Games;
                return tovPg.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        // Steal
        public string StlPG
        {
            get
            {
                double stlPg = (double)Stl / Games;
                return stlPg.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        // Block
        public string BlkPG
        {
            get
            {
                double blkPg = (double)Blk / Games;
                return blkPg.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        // Fouls
        public string PfPG
        {
            get
            {
                double pfPg = (double)Pf / Games;
                return pfPg.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        //FgAtRimPct
        public string FgAtRimPct
        {
            get
            {
                if (FgaAtRim == 0)
                {
                    return "0.0";
                }

                double percentage = (double)FgAtRim / FgaAtRim * 100;
                return percentage.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        //FgLowPostPct
        public string FgLowPostPct
        {
            get
            {
                if (FgaLowPost == 0)
                {
                    return "0.0";
                }

                double percentage = (double)FgLowPost / FgaLowPost * 100;
                return percentage.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        //FgMidRangePct
        public string FgMidRangePct
        {
            get
            {
                if (FgaMidRange == 0)
                {
                    return "0.0";
                }

                double percentage = (double)FgMidRange / FgaMidRange * 100;
                return percentage.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }



        //Points
        public string PtsPG
        {
            get
            {
                double PtsPg = (double)Pts / Games;
                return PtsPg.ToString("0.0", CultureInfo.InvariantCulture);

            }
        }

        public string PER
        {
            get
            {
                double per = (1.0 / Min) * ((Pts * 85.910) + (Trb * 53.840) + (Ast * 34.677) + (Stl * 53.840) + (Blk * 53.840) - (Fga * 39.190) - (Fta * 20.091) - (Tov * 53.840) - (Pf * 17.201));
                return per.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        public string TrueShooting
        {
            get
            {
                double tsa = (double)(Pts + (0.44 * Fta));
                double ts = (double)(Pts / (2 * tsa));
                return ts.ToString("00.0", CultureInfo.InvariantCulture);
            }
        }


    }

}
