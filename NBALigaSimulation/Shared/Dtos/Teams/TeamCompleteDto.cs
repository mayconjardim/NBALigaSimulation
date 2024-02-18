using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Shared.Dtos.Teams
{
    public class TeamCompleteDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Abrv { get; set; } = string.Empty;
        public string Conference { get; set; } = string.Empty;
        public List<PlayerCompleteDto> Players { get; set; } = new List<PlayerCompleteDto>();
        public List<TeamDraftPickDto> DraftPicks { get; set; } = new List<TeamDraftPickDto>();
        public TeamGameplanDto Gameplan { get; set; }
    }
}
