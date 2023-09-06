using System.ComponentModel.DataAnnotations.Schema;

namespace NBALigaSimulation.Shared.Models
{
    [NotMapped]
    public class TeamCompositeRating
    {

        public Dictionary<string, decimal> Ratings { get; set; }

        public TeamCompositeRating()
        {
            Ratings = new Dictionary<string, decimal>();
        }

    }
}
