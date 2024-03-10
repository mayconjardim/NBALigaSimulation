namespace NBALigaSimulation.Client.Utilities
{
    public static class Util
    {

        public static int Age(int season, int born)
        {
            return season - born;
        }
        
        public static int InchesToCm(double inches)
        {
            const double cmPerInch = 2.54;
            return (int)(inches * cmPerInch);
        }

        public static int LbsToKg(double lbs)
        {
            const double kgPerLbs = 0.45359237;
            return (int)(lbs * kgPerLbs);
        }

        public static string Position(string pos)
        {
            switch (pos)
            {
                case "PG":
                    return "Point Guard";
                case "SG":
                    return "Shooting Guard";
                case "SF":
                    return "Small Forward";
                case "PF":
                    return "Power Forward";
                case "C":
                    return "Center";
                case "G":
                    return "Guard";
                case "GF":
                    return "Guard/Forward";
                case "F":
                    return "Forward";
                case "FC":
                    return "Forward/Center";
                default:
                    return string.Empty;
            }
        }

        public static string GetOrdinal(int x)
        {
            string suffix;

            if (x >= 11 && x <= 13)
            {
                suffix = "th";
            }
            else if (x % 10 == 1)
            {
                suffix = "st";
            }
            else if (x % 10 == 2)
            {
                suffix = "nd";
            }
            else if (x % 10 == 3)
            {
                suffix = "rd";
            }
            else
            {
                suffix = "th";
            }

            return x.ToString() + suffix;
        }

        public static string GetScoreClass(int homeScore, int awayScore)
        {
            if (homeScore > awayScore)
            {
                return "text-green";
            }
            else if (awayScore > homeScore)
            {
                return "text-red";
            }
            else
            {
                return "";
            }
        }


    }

}
