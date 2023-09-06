using System.ComponentModel.DataAnnotations.Schema;

namespace NBALigaSimulation.Shared.Models
{
    [NotMapped]
    public class TeamSynergy
    {

        public decimal Off { get; set; }
        public decimal Def { get; set; }
        public decimal Reb { get; set; }

    }
}
