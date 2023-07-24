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

        public double GamePace
        {
            get
            {
                double pace = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("spd", Spd);
                rating.Add("jmp", Jmp);
                rating.Add("dnk", Dnk);
                rating.Add("tp", Tp);
                rating.Add("stl", Stl);
                rating.Add("drb", Reb);
                rating.Add("pss", Pss);
                List<string> attributes = new List<string> { "spd", "jmp", "dnk", "tp", "stl", "drb", "pss" };

                pace = Converter.Composite(rating, attributes);
                return pace;
            }
        }

        public double GameUsage
        {
            get
            {
                double usage = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("ins", Ins);
                rating.Add("dnk", Dnk);
                rating.Add("fg", Fg);
                rating.Add("tp", Tp);
                rating.Add("spd", Spd);
                rating.Add("drb", Drb);

                List<string> attributes = new List<string> { "ins", "dnk", "fg", "tp", "spd", "drb" };
                List<double> weights = new List<double> { 1.5, 1, 1, 1, 0.15, 0.15 };

                usage = Converter.Composite(rating, attributes, weights);
                return usage;
            }
        }

        public double GameDribbling
        {
            get
            {
                double dribbling = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("drb", Drb);
                rating.Add("spd", Spd);

                List<string> attributes = new List<string> { "drb", "spd" };

                dribbling = Converter.Composite(rating, attributes);
                return dribbling;
            }
        }

        public double GamePassing
        {
            get
            {
                double passing = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("drb", Drb);
                rating.Add("pss", Pss);

                List<string> attributes = new List<string> { "drb", "pss" };
                List<double> weights = new List<double> { 0.4, 1 };

                passing = Converter.Composite(rating, attributes, weights);
                return passing;
            }
        }

        public double GameTurnovers
        {
            get
            {
                double turnovers = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("drb", Drb);
                rating.Add("pss", Pss);
                rating.Add("spd", Spd);
                rating.Add("hgt", Hgt);
                rating.Add("ins", Ins);

                List<string> attributes = new List<string> { "drb", "pss", "spd", "hgt", "ins" };
                List<double> weights = new List<double> { 1, 1, -1, 1, 1 };

                turnovers = Converter.Composite(rating, attributes, weights);
                return turnovers;
            }
        }

        public double GameShootingAtRim
        {
            get
            {
                double shootingAtRim = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("hgt", Hgt);
                rating.Add("spd", Spd);
                rating.Add("jmp", Jmp);
                rating.Add("dnk", Dnk);

                List<string> attributes = new List<string> { "hgt", "spd", "jmp", "dnk" };
                List<double> weights = new List<double> { 1, 0.2, 0.6, 0.4 };

                shootingAtRim = Converter.Composite(rating, attributes, weights);
                return shootingAtRim;
            }
        }

        public double GameShootingLowPost
        {
            get
            {
                double shootingLowPost = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("hgt", Hgt);
                rating.Add("stre", Str);
                rating.Add("spd", Spd);
                rating.Add("ins", Ins);

                List<string> attributes = new List<string> { "hgt", "stre", "spd", "ins" };
                List<double> weights = new List<double> { 1, 0.6, 0.2, 1 };

                shootingLowPost = Converter.Composite(rating, attributes, weights);
                return shootingLowPost;
            }
        }

        public double GameShootingMidRange
        {
            get
            {
                double shootingMidRange = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("hgt", Hgt);
                rating.Add("fg", Fg);

                List<string> attributes = new List<string> { "hgt", "fg" };
                List<double> weights = new List<double> { 0.2, 1 };

                shootingMidRange = Converter.Composite(rating, attributes, weights);
                return shootingMidRange;
            }
        }

        public double GameShootingThreePointer
        {
            get
            {
                double shootingThreePointer = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("hgt", Hgt);
                rating.Add("tp", Tp);

                List<string> attributes = new List<string> { "hgt", "tp" };
                List<double> weights = new List<double> { 0.2, 1 };

                shootingThreePointer = Converter.Composite(rating, attributes, weights);
                return shootingThreePointer;
            }
        }

        public double GameShootingFT
        {
            get
            {
                double shootingFT = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("ft", Ft);

                List<string> attributes = new List<string> { "ft" };

                shootingFT = Converter.Composite(rating, attributes);
                return shootingFT;
            }
        }

        public double GameRebounding
        {
            get
            {
                double rebounding = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("hgt", Hgt);
                rating.Add("stre", Str);
                rating.Add("jmp", Jmp);
                rating.Add("reb", Reb);

                List<string> attributes = new List<string> { "hgt", "stre", "jmp", "reb" };
                List<double> weights = new List<double> { 1.5, 0.1, 0.1, 0.7 };

                rebounding = Converter.Composite(rating, attributes, weights);
                return rebounding;
            }
        }

        public double GameStealing
        {
            get
            {
                double stealing = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("spd", Spd);
                rating.Add("stl", Stl);

                List<string> attributes = new List<string> { "spd", "stl" };


                stealing = Converter.Composite(rating, attributes);
                return stealing;
            }
        }

        public double GameBlocking
        {
            get
            {
                double blocking = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("hgt", Hgt);
                rating.Add("jmp", Jmp);
                rating.Add("blk", Blk);

                List<string> attributes = new List<string> { "hgt", "jmp", "blk" };
                List<double> weights = new List<double> { 1.5, 0.5, 0.5 };


                blocking = Converter.Composite(rating, attributes, weights);
                return blocking;
            }
        }

        public double GameFouling
        {
            get
            {
                double fouling = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("hgt", Hgt);
                rating.Add("blk", Blk);
                rating.Add("spd", Spd);

                List<string> attributes = new List<string> { "hgt", "blk", "spd" };
                List<double> weights = new List<double> { 1.5, 1, 1, -1 };


                fouling = Converter.Composite(rating, attributes, weights);
                return fouling;
            }
        }

        public double GameDefense
        {
            get
            {
                double defense = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("hgt", Hgt);
                rating.Add("stre", Str);
                rating.Add("spd", Spd);
                rating.Add("jmp", Jmp);
                rating.Add("blk", Blk);
                rating.Add("stl", Stl);

                List<string> attributes = new List<string> { "hgt", "stre", "spd", "jmp", "blk", "stl" };
                List<double> weights = new List<double> { 1, 1, 1, 0.5, 1, 1 };


                defense = Converter.Composite(rating, attributes, weights);
                return defense;
            }
        }

        public double GameDefenseInterior
        {
            get
            {
                double defenseInterior = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("hgt", Hgt);
                rating.Add("stre", Str);
                rating.Add("spd", Spd);
                rating.Add("jmp", Jmp);
                rating.Add("blk", Blk);

                List<string> attributes = new List<string> { "hgt", "stre", "spd", "jmp", "blk" };
                List<double> weights = new List<double> { 2, 1, 0.5, 0.5, 1 };


                defenseInterior = Converter.Composite(rating, attributes, weights);
                return defenseInterior;
            }
        }

        public double GameDefensePerimeter
        {
            get
            {
                double defensePerimeter = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("hgt", Hgt);
                rating.Add("stre", Str);
                rating.Add("spd", Spd);
                rating.Add("jmp", Jmp);
                rating.Add("stl", Stl);

                List<string> attributes = new List<string> { "hgt", "stre", "spd", "jmp", "stl" };
                List<double> weights = new List<double> { 1, 1, 2, 0.5, 1 };


                defensePerimeter = Converter.Composite(rating, attributes, weights);
                return defensePerimeter;
            }
        }

        public double GameEndurance
        {
            get
            {
                double endurance = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("endu", End);
                rating.Add("hgt", Hgt);


                List<string> attributes = new List<string> { "endu", "hgt" };
                List<double> weights = new List<double> { 1, 1, -0.1 };


                endurance = Converter.Composite(rating, attributes, weights);
                return endurance;
            }
        }

        public double GameAthleticism
        {
            get
            {
                double athleticism = 0;

                Dictionary<string, double> rating = new Dictionary<string, double>();
                rating.Add("stre", Str);
                rating.Add("spd", Spd);
                rating.Add("jmp", Jmp);
                rating.Add("hgt", Hgt);


                List<string> attributes = new List<string> { "stre", "spd", "jmp", "hgt" };
                List<double> weights = new List<double> { 1, 1, 1, 0.5 };


                athleticism = Converter.Composite(rating, attributes, weights);
                return athleticism;
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

    }
}
