
namespace NBALigaSimulation.Shared.Dtos
{

    public class TeamCompositeDto
    {

        public Dictionary<string, double> CompositeRating { get; set; }

        public TeamCompositeDto()
        {
            CompositeRating = new Dictionary<string, double>();
        }

    }
}
