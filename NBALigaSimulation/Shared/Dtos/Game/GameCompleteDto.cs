namespace NBALigaSimulation.Shared.Dtos
{
    public class GameCompleteDto
    {

        public int Id { get; set; }
        public string HomeTeam { get; set; }
        public int HomeTeamId { get; set; }
        public string AwayTeam { get; set; }
        public int AwayTeamId { get; set; }
        public int Season { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public DateTime GameDate { get; set; }
        public List<TeamGameStatsDto> TeamGameStats { get; set; }
        public List<PlayerGameStatsDto> HomePlayerGameStats { get; set; }
        public List<PlayerGameStatsDto> AwayPlayerGameStats { get; set; }

    }
}
