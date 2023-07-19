namespace NBALigaSimulation.Shared.Dtos
{
    public class DraftPlayerDto
    {

        public int PlayerId { get; set; }
        public int TeamId { get; set; }
        public string Team { get; set; }
        public int Year { get; set; }
        public int Round { get; set; }
        public int Pick { get; set; }

    }
}
