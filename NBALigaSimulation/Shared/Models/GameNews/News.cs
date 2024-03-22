namespace NBALigaSimulation.Shared.Models.GameNews
{
    public class News
    {

        public int Id { get; set; }
        public int? GameId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Winner { get; set; } = string.Empty;


    }
}
