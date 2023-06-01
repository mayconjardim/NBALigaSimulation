using NBALigaSimulation.Shared.Engine;
using NBALigaSimulation.Shared.Models;
using System.Net;

namespace NBALigaSimulation.Shared.Dtos
{
    public class PlayerRatingDto
    {
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
        public int Ovr => (int)Math.Round((4 * Hgt + Str + 4 * Spd + 2 * Jmp + 3 * End + 3 * Ins + 4 * Dnk + Ft + Fg + 2 * Tp + Blk + Stl + Drb + 3 * Pss + Reb) / 32.0);

        public string Position
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

                if (Converter.hasSkill(new List<int> { Hgt, Tp }, new List<double> { 0.2, 1.0 }))
                {
                    skills.Add("3");
                }
                if (Converter.hasSkill(new List<int> { Str, Spd, Jmp, Hgt }, new List<double> { 1, 1, 1, 0.5 }))
                {
                    skills.Add("A");
                }
                if (Converter.hasSkill(new List<int> { Drb, Spd }, new List<double> { 1, 1 }))
                {
                    skills.Add("B");
                }
                if (Converter.hasSkill(new List<int> { Hgt, Str, Spd, Jmp, Blk }, new List<double> { 2, 1, 0.5, 0.5, 1 }))
                {
                    skills.Add("Di");
                }
                if (Converter.hasSkill(new List<int> { Hgt, Str, Spd, Jmp, Stl }, new List<double> { 1, 1, 2, 0.5, 1 }))
                {
                    skills.Add("Dp");
                }
                if (Converter.hasSkill(new List<int> { Hgt, Str, Spd, Ins }, new List<double> { 1, 0.6, 0.2, 1 }))
                {
                    skills.Add("Po");
                }
                if (Converter.hasSkill(new List<int> { Drb, Pss }, new List<double> { 0.4, 1 }))
                {
                    skills.Add("Ps");
                }
                if (Converter.hasSkill(new List<int> { Hgt, Str, Jmp, Reb }, new List<double> { 1, 0.1, 0.1, 0.7 }))
                {
                    skills.Add("R");
                }

                return skills;
            }
        }

    }
}
