using NBALigaSimulation.Shared.Models;

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
        public List<PlayerRatingDto> Ratings { get; set; } = new List<PlayerRatingDto>();
    }
}
