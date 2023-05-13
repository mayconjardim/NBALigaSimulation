using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace NBALigaSimulation.Shared.Models
{
    public class TeamGameStats
    {

        public int Id { get; set; }
        [JsonIgnore]
        public int GameId { get; set; }
        public int TeamId { get; set; }
        public int Season { get; set; }
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
        public int Trb { get; set; }
        public int Ast { get; set; }
        public int Tov { get; set; }
        public int Stl { get; set; }
        public int Blk { get; set; }
        public int Pf { get; set; }
        public int Pts { get; set; }

        [NotMapped]
        public List<int> PtsQtrs { get; set; }

        [Column("PtsQtrs")]
        public string PtsQtrsJson
        {
            get => JsonConvert.SerializeObject(PtsQtrs);
            set => PtsQtrs = JsonConvert.DeserializeObject<List<int>>(value);
        }

    }
}
