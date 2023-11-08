namespace NBALigaSimulation.Shared.Models
{
    public class News
    {

        public int Id { get; set; }
        public int? GameId { get; set; }
        public string Title { get; set; } = string.Empty;

    }
}
