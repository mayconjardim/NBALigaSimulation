using System.Text.Json.Serialization;

namespace NBALigaSimulation.Shared.Models
{
    public class PlayerRatings
    {

        [JsonIgnore]
        public Player Player { get; set; }
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public int hgt { get; set; }
        public int str { get; set; }
        public int spd { get; set; }
        public int jmp { get; set; }
        public int end { get; set; }
        public int ins { get; set; }
        public int dnk { get; set; }
        public int ft { get; set; }
        public int fg { get; set; }
        public int tp { get; set; }
        public int blk { get; set; }
        public int stl { get; set; }
        public int drb { get; set; }
        public int pss { get; set; }
        public int reb { get; set; }

    }
}
