namespace NBALigaSimulation.Shared.Dtos.Teams
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
