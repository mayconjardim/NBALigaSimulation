using System.ComponentModel.DataAnnotations.Schema;

namespace NBALigaSimulation.Shared.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
        public int BornYear { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public Team Team { get; set; }
        public int TeamId { get; set; }
        public int PtModifier { get; set; }
        public int RosterOrder { get; set; }
        public List<PlayerRatings> Ratings { get; set; } = new List<PlayerRatings>();
        public List<PlayerGameStats> Stats { get; set; } = new List<PlayerGameStats>();
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

    }
}
