using System.ComponentModel.DataAnnotations.Schema;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Models.Players
{
    public class Player
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Pos { get; set; } = string.Empty;
        public Born Born { get; set; }
        public PlayerDraft? Draft { get; set; }
        public string College { get; set; } = string.Empty;
        public int Hgt { get; set; }
        public int Weight { get; set; }
        public string ImgUrl { get; set; } = string.Empty;
        public Team Team { get; set; }
        public int TeamId { get; set; }
        public double PtModifier { get; set; }
        public int RosterOrder { get; set; }
        public bool KeyPlayer { get; set; } = false;
        public PlayerContract? Contract { get; set; }
        public List<PlayerRatings> Ratings { get; set; } = new List<PlayerRatings>();
        public List<PlayerGameStats> Stats { get; set; } = new List<PlayerGameStats>();
        public List<PlayerRegularStats> RegularStats { get; set; } = new List<PlayerRegularStats>();
        public List<PlayerPlayoffsStats> PlayoffsStats { get; set; } = new List<PlayerPlayoffsStats>();
        public List<PlayerAwards> PlayerAwards { get; set; } = new List<PlayerAwards>();

        [NotMapped]
        public PlayerCompositeRating CompositeRating { get; set; }

    }
}
