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

    }
}
