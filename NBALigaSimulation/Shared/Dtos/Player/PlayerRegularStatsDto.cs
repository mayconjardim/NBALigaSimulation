using System.Runtime.Intrinsics.X86;

namespace NBALigaSimulation.Shared.Dtos
{
    public class PlayerRegularStatsDto
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int PlayerId { get; set; }
        public int TeamId { get; set; }
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

        public double MinPG
        {
            get
            {
                return (double)(Min / Games);
            }
        }

        //FG
        public double FgPG
        {
            get
            {
                return (double)(Fg / Games);
            }
        }

        public double FgaPG
        {
            get
            {
                return (double)(Fga / Games);
            }
        }

        public double FgPct
        {
            get
            {
                if (Fga == 0)
                {
                    return 0.0;
                }

                double percentage = (double)Fg / Fga * 100;
                return percentage;
            }
        }

        //3PT
        public double TpPG
        {
            get
            {
                return (double)(Tp / Games);
            }
        }

        public double TpaPG
        {
            get
            {
                return (double)(Tpa / Games);
            }
        }

        public double TpPct
        {
            get
            {
                if (Tpa == 0)
                {
                    return 0.0;
                }

                double percentage = (double)Tp / Tpa * 100;
                return percentage;
            }
        }

        //FT
        public double FtPG
        {
            get
            {
                return (double)(Ft / Games);
            }
        }

        public double FtaPG
        {
            get
            {
                return (double)(Fta / Games);
            }
        }

        public double FtPct
        {
            get
            {
                if (Fta == 0)
                {
                    return 0.0;
                }

                double percentage = (double)Ft / Fta * 100;
                return percentage;
            }
        }

        //Reb
        public double ORebPG
        {
            get
            {
                return (double)(Orb / Games);
            }
        }

        public double DRebPG
        {
            get
            {
                return (double)(Drb / Games);
            }
        }

        public double TRebPG
        {
            get
            {
                return (double)(Trb / Games);
            }
        }

        //Ast
        public double AstPG
        {
            get
            {
                return (double)(Ast / Games);
            }
        }

        //TO
        public double TovPG
        {
            get
            {
                return (double)(Tov / Games);
            }
        }

        //Steal
        public double StlPG
        {
            get
            {
                return (double)(Stl / Games);
            }
        }

        //Block
        public double BlkPG
        {
            get
            {
                return (double)(Blk / Games);
            }
        }

        //Fouls
        public double PfPG
        {
            get
            {
                return (double)(Pf / Games);
            }
        }

        //Points
        public double PtsPG
        {
            get
            {
                return (double)(Pts / Games);

            }
        }

        public double PER
        {
            get
            {
                double per = (1.0 / Min) * ((Pts * 85.910) + (Trb * 53.840) + (Ast * 34.677) + (Stl * 53.840) + (Blk * 53.840) - (Fga * 39.190) - (Fta * 20.091) - (Tov * 53.840) - (Pf * 17.201));
                return per;
            }
        }

        public double TrueShooting
        {
            get
            {
                double tsa = (double)(Pts + (0.44 * Fta));
                double ts = (double)(Pts / (2 * tsa));
                return ts;
            }
        }


    }

}
