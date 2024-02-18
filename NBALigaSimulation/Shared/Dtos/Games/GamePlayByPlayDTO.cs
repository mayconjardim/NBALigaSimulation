namespace NBALigaSimulation.Shared.Dtos.Games
{
    public class GamePlayByPlayDTO
    {
        public int? Id { get; set; }
        public int? GameSimId { get; set; }
        public string? Type { get; set; }
        public string? Text { get; set; }
        public string? TeamName { get; set; }
        public string? Score { get; set; }
        public double? T { get; set; }
        public string? Time { get; set; }
        public int? On { get; set; }
        public int? Off { get; set; }
    }
}
