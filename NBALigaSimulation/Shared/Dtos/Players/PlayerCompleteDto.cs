using System.ComponentModel;
using NBALigaSimulation.Shared.Models.Players;
using Newtonsoft.Json;

namespace NBALigaSimulation.Shared.Dtos.Players
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
        public bool KeyPlayer { get; set; } = false;

        [JsonProperty("RosterOrder")]
        public int RosterOrder { get; set; }
        public PlayerDraft? Draft { get; set; }
        public PlayerContractDto? Contract { get; set; }
        public List<PlayerRatingDto> Ratings { get; set; } 
        public List<PlayerRegularStatsDto> RegularStats { get; set; } 
        public List<PlayerPlayoffsStatsDto> PlayoffsStats { get; set; } 
        
        [JsonProperty("GameLogs")]
        public List<PlayerGameStatsDto> Stats { get; set; } 
        public List<PlayerAwardsDto> PlayerAwards { get; set; } 

    }
}
