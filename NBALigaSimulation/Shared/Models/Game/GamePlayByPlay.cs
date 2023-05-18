namespace NBALigaSimulation.Shared.Models
{
    public class GamePlayByPlay
    {

        public int Id { get; set; }
        public int GameSimId { get; set; }
        public Game Game { get; set; }
        public int Sequence { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

    }
}
