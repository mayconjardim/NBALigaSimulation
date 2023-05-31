using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Shared.Dtos
{
    public class CreatePlayerDto
    {

        public string Name { get; set; } = string.Empty;
        public string Pos { get; set; } = string.Empty;
        public Born Born { get; set; }
        public int Hgt { get; set; }
        public int Weight { get; set; }
        public string ImgUrl { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public List<PlayerRatingDto> Ratings { get; set; } = new List<PlayerRatingDto>();

    }
}
