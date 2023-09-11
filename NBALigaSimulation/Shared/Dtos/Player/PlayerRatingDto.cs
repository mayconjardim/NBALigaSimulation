using NBALigaSimulation.Shared.Engine;

namespace NBALigaSimulation.Shared.Dtos
{
    public class PlayerRatingDto
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public int Diq { get; set; }
        public int Dnk { get; set; }
        public int Drb { get; set; }
        public int Endu { get; set; }
        public int Fg { get; set; }
        public int Ft { get; set; }
        public int Fuzz { get; set; }
        public int Hgt { get; set; }
        public int Ins { get; set; }
        public int Jmp { get; set; }
        public int Oiq { get; set; }
        public string Pos { get; set; }
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

        public List<string> Skills
        {
            get
            {
                List<string> skills = new();

                if (Converter.hasSkill(new List<int> { Hgt, Tp }, new List<double> { 0.2, 1.0 }))
                {
                    skills.Add("3");
                }
                if (Converter.hasSkill(new List<int> { Stre, Spd, Jmp, Hgt }, new List<double> { 1, 1, 1, 0.5 }))
                {
                    skills.Add("A");
                }
                if (Converter.hasSkill(new List<int> { Drb, Spd }, new List<double> { 1, 1 }))
                {
                    skills.Add("B");
                }
                if (Converter.hasSkill(new List<int> { Hgt, Stre, Spd, Jmp, Diq }, new List<double> { 2, 1, 0.5, 0.5, 1 }))
                {
                    skills.Add("Di");
                }
                if (Converter.hasSkill(new List<int> { Hgt, Stre, Spd, Jmp, Diq }, new List<double> { 1, 1, 2, 0.5, 1 }))
                {
                    skills.Add("Dp");
                }
                if (Converter.hasSkill(new List<int> { Hgt, Stre, Spd, Ins }, new List<double> { 1, 0.6, 0.2, 1 }))
                {
                    skills.Add("Po");
                }
                if (Converter.hasSkill(new List<int> { Drb, Pss }, new List<double> { 0.4, 1 }))
                {
                    skills.Add("Ps");
                }
                if (Converter.hasSkill(new List<int> { Hgt, Stre, Jmp, Reb }, new List<double> { 1, 0.1, 0.1, 0.7 }))
                {
                    skills.Add("R");
                }

                return skills;
            }
        }
    }

}
