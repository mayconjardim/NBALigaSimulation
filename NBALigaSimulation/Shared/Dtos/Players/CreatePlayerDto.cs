using NBALigaSimulation.Shared.Models.Players;

namespace NBALigaSimulation.Shared.Dtos.Players
{
    public class CreatePlayerDto
    {
        
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Pos { get; set; } = string.Empty;
        public string College { get; set; } = string.Empty;

        public Born Born { get; set; }
        public int Hgt { get; set; }
        public int Weight { get; set; }
        public string ImgUrl { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public List<PlayerRatingDto> Ratings { get; set; } = new List<PlayerRatingDto>();

    }
}
