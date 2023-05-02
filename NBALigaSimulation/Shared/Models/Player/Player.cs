using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public Team Team { get; set; }
        public int TeamId { get; set; }
        public List<PlayerRatings> Ratings { get; set; } = new List<PlayerRatings>();

    }
}
