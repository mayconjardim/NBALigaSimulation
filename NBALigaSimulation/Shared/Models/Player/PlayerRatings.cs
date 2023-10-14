using NBALigaSimulation.Shared.Engine;
using System.Text.Json.Serialization;

namespace NBALigaSimulation.Shared.Models
{
    public class PlayerRatings
    {
        public int Id { get; set; }
        [JsonIgnore]
        public Player Player { get; set; }
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public int Diq { get; set; }
        public int Dnk { get; set; }
        public int Drb { get; set; }
        public int Endu { get; set; }
        public int Fg { get; set; }
        public int Ft { get; set; }
        public int Hgt { get; set; }
        public int Ins { get; set; }
        public int Jmp { get; set; }
        public int Oiq { get; set; }
        public int Pot { get; set; }
        public int Pss { get; set; }
        public int Reb { get; set; }
        public int Spd { get; set; }
        public int Stre { get; set; }
        public int Tp { get; set; }

        public int CalculateOvr
        {
            get
            {
                double r =
                  (5 * Hgt +
                  1 * Stre +
                  4 * Spd +
                  2 * Jmp +
                  1 * Endu +
                  1 * Ins +
                  2 * Dnk +
                  1 * Ft +
                  1 * Fg +
                  3 * Tp +
                  7 * Oiq +
                  3 * Diq +
                  3 * Drb +
                  3 * Pss +
                  1 * Reb) / 38;

                double fudgeFactor = 0;
                if (r >= 68)
                {
                    fudgeFactor = 8;
                }
                else if (r >= 50)
                {
                    fudgeFactor = 4 + (r - 50) * (4 / 18);
                }
                else if (r >= 42)
                {
                    fudgeFactor = -5 + (r - 42) * (10 / 8);
                }
                else if (r >= 31)
                {
                    fudgeFactor = -5 - (42 - r) * (5 / 11);
                }
                else
                {
                    fudgeFactor = -10;
                }

                return RandomUtils.Bound((int)Math.Round(r + fudgeFactor), 0, 100);
            }
        }


    }
}
