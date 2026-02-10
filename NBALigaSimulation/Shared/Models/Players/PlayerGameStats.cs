using NBALigaSimulation.Shared.Models.Games;

namespace NBALigaSimulation.Shared.Models.Players
{
    public class PlayerGameStats
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Pos { get; set; } = string.Empty;
        public int PlayerId { get; set; }
        public Player? Player { get; set; }
        public int TeamId { get; set; }
        public string OppAbrev { get; set; } = string.Empty;
        public DateTime GameDate { get; set; }
        public int GameId { get; set; }
        public int Season { get; set; }
        public Game Game { get; set; }
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
        public double CourtTime { get; set; }
        public double BenchTime { get; set; }
        public double Energy { get; set; }
        
        public double GameScore
        {
            get
            {
                double gs = (double)Pts + (0.4 * Fg) - (0.7 * Fga) - (0.4 * (Fta - Ft)) + (0.7 * Orb) +
                    (0.3 * Drb) + Stl + (0.7 * Ast) + (0.7 * Blk) - (0.4 * Pf) - Tov;
                return gs;
            }
        }
        
    }
}



