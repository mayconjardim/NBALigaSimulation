using NBALigaSimulation.Shared.Models;
using Newtonsoft.Json;

namespace NBALigaSimulation.Shared.Dtos
{
    public class PlayerCompleteDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Pos { get; set; } = string.Empty;
        public Born Born { get; set; }
        public int Hgt { get; set; }
        public int Weight { get; set; }
        public string ImgUrl { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string TeamAbrv { get; set; } = string.Empty;
        public double PtModifier { get; set; }

        [JsonProperty("RosterOrder")]
        public int RosterOrder { get; set; }
        public PlayerDraft? Draft { get; set; }
        public PlayerContractDto? Contract { get; set; }
        public List<PlayerRatingDto> Ratings { get; set; } = new List<PlayerRatingDto>();
        public List<PlayerRegularStatsDto> RegularStats { get; set; } = new List<PlayerRegularStatsDto>();
        public List<PlayerPlayoffsStatsDto> PlayoffsStats { get; set; } = new List<PlayerPlayoffsStatsDto>();
        public List<PlayerGameStatsDto> Stats { get; set; } = new List<PlayerGameStatsDto>();
        public List<PlayerAwardsDto> PlayerAwards { get; set; } = new List<PlayerAwardsDto>();

    }
}
