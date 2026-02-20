using System.Globalization;

namespace NBALigaSimulation.Client.Utilities
{
    public static class Util
    {

        public static int Age(int season, int born)
        {
            // Garante que a temporada e o ano de nascimento sejam válidos
            if (season <= 0 || born <= 0)
            {
                // Se não temos dados válidos, retorna uma idade padrão baseada no ano atual
                var currentYear = DateTime.Now.Year;
                if (born > 0)
                {
                    return Math.Max(18, currentYear - born); // Mínimo de 18 anos para jogadores
                }
                return 20; // Idade padrão se não houver dados
            }
            
            var age = season - born;
            // Garante que a idade seja sempre positiva e razoável (entre 18 e 50 anos)
            return Math.Max(18, Math.Min(50, age));
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

        public static string GetBadgeClass(int number)
        {
            switch (number)
            {
                case int n when n > 80:
                    return "rating-roxo";
                case int n when n > 60:
                    return "rating-azul";
                case int n when n > 50:
                    return "rating-verde";
                case int n when n > 40:
                    return "rating-amarelo";
                case int n when n > 30:
                    return "rating-laranja";
                default:
                    return "rating-vermelho";
            }
        }

        public static string FormatNumber(double numero)
        {
            string numeroFormatado = numero.ToString("0.0", CultureInfo.InvariantCulture);
            return numeroFormatado;
        }

        public static string GetRatingColor(int value1, int value2)
        {
            if (value1 > value2)
            {
                return "rating-up";
            }
            else
            {
                return "rating-down";
            }
        }

        public static string NomeAbvr(string nomeCompleto)
        {
            string[] partesNome = nomeCompleto.Split(' ');

            if (partesNome.Length < 2)
            {
                return nomeCompleto;
            }

            string primeiroNome = partesNome[0];
            string sobrenome = partesNome[partesNome.Length - 1];

            string primeiraLetra = primeiroNome.Substring(0, 1);

            return $"{primeiraLetra}. {sobrenome}";
        }
        
        public static string GetPtBackground(double value)
        {
            string background = string.Empty;
    
            switch (value)
            {
                case 0.0:
                    background = "red";
                    break;
                case 0.75:
                    background = "yellow";
                    break;
                case 1.25:
                    background = "blue";
                    break;
                case 1.75:
                    background = "greenyellow";
                    break;
                case 2.0:
                    background = "green";
                    break;
                default:
                    background = "white";
                    break;
            }

            return background;
        }

        public static string FormatStat(double stat, string format = "F1")
        {
            if (double.IsNaN(stat) || double.IsInfinity(stat))
            {
                return "0.0";
            }
            return stat.ToString(format, CultureInfo.InvariantCulture);
        }

    }
}