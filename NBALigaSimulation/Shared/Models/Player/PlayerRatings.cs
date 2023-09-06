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
        public int Hgt { get; set; }
        public int Str { get; set; }
        public int Spd { get; set; }
        public int Jmp { get; set; }
        public int End { get; set; }
        public int Ins { get; set; }
        public int Dnk { get; set; }
        public int Ft { get; set; }
        public int Fg { get; set; }
        public int Tp { get; set; }
        public int Blk { get; set; }
        public int Stl { get; set; }
        public int Drb { get; set; }
        public int Pss { get; set; }
        public int Reb { get; set; }
        public int Pot { get; set; }

        public int Ovr
        {
            get
            {
                int ovr = (int)Math.Round((4 * Hgt + Str + 4 * Spd + 2 * Jmp + 3 * End + 3 * Ins + 4 * Dnk + Ft + Fg + 2 * Tp + Blk + Stl + Drb + 3 * Pss + Reb) / 32.0);
                return ovr;
            }
        }

        public string Pos
        {
            get
            {
                bool c = false, g = false, pf = false, pg = false, sf = false, sg = false;
                string position;

                if (Reb >= 50)
                {
                    position = "GF";
                }
                else
                {
                    position = "F";
                }

                if (Hgt <= 30 || Spd >= 85)
                {
                    g = true;
                    if ((Pss + Reb) >= 100)
                    {
                        pg = true;
                    }
                    if (Hgt >= 30)
                    {
                        sg = true;
                    }
                }

                if (Hgt >= 50 && Hgt <= 65 && Spd >= 40)
                {
                    sf = true;
                }

                if (Hgt >= 70)
                {
                    pf = true;
                }

                if ((Hgt + Str) >= 130)
                {
                    c = true;
                }

                if (pg && !sg && !sf && !pf && !c)
                {
                    position = "PG";
                }
                else if (!pg && (g || sg) && !sf && !pf && !c)
                {
                    position = "SG";
                }
                else if (!pg && !sg && sf && !pf && !c)
                {
                    position = "SF";
                }
                else if (!pg && !sg && !sf && pf && !c)
                {
                    position = "PF";
                }
                else if (!pg && !sg && !sf && !pf && c)
                {
                    position = "C";
                }


                if ((pf || sf) && g)
                {
                    position = "GF";
                }
                else if (c && (pf || sf))
                {
                    position = "FC";
                }
                else if (pg && sg)
                {
                    position = "G";
                }

                if (position == "F" && Reb <= 20)
                {
                    position = "PF";
                }

                return position;

            }
        }

        public List<string> Skills
        {
            get
            {
                List<string> skills = new();

                if (Converter.hasSkill(new List<int> { Hgt, Tp }, new List<decimal> { 0.2m, 1.0m }))
                {
                    skills.Add("3");
                }
                if (Converter.hasSkill(new List<int> { Str, Spd, Jmp, Hgt }, new List<decimal> { 1, 1, 1, 0.5m }))
                {
                    skills.Add("A");
                }
                if (Converter.hasSkill(new List<int> { Drb, Spd }, new List<decimal> { 1, 1 }))
                {
                    skills.Add("B");
                }
                if (Converter.hasSkill(new List<int> { Hgt, Str, Spd, Jmp, Blk }, new List<decimal> { 2, 1, 0.5m, 0.5m, 1 }))
                {
                    skills.Add("Di");
                }
                if (Converter.hasSkill(new List<int> { Hgt, Str, Spd, Jmp, Stl }, new List<decimal> { 1, 1, 2, 0.5m, 1 }))
                {
                    skills.Add("Dp");
                }
                if (Converter.hasSkill(new List<int> { Hgt, Str, Spd, Ins }, new List<decimal> { 1, 0.6m, 0.2m, 1 }))
                {
                    skills.Add("Po");
                }
                if (Converter.hasSkill(new List<int> { Drb, Pss }, new List<decimal> { 0.4m, 1 }))
                {
                    skills.Add("Ps");
                }
                if (Converter.hasSkill(new List<int> { Hgt, Str, Jmp, Reb }, new List<decimal> { 1, 0.1m, 0.1m, 0.7m }))
                {
                    skills.Add("R");
                }

                return skills;
            }
        }

    }
}
