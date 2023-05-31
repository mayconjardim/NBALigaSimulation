using System.ComponentModel.DataAnnotations.Schema;

namespace NBALigaSimulation.Shared.Models
{
    public class Player
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Pos { get; set; } = string.Empty;
        public Born Born { get; set; }
        public int Hgt { get; set; }
        public int Weight { get; set; }
        public string ImgUrl { get; set; } = string.Empty;
        public Team Team { get; set; }
        public int TeamId { get; set; }
        public double PtModifier { get; set; }
        public int RosterOrder { get; set; }
        public List<PlayerRatings> Ratings { get; set; } = new List<PlayerRatings>();
        public List<PlayerGameStats> Stats { get; set; } = new List<PlayerGameStats>();

    }
}
