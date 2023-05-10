
namespace NBALigaSimulation.Shared.Dtos
{
    public class TeamCompleteDto
    {

        public string Name { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Abrv { get; set; } = string.Empty;
        public List<PlayerCompleteDto> Players { get; set; } = new List<PlayerCompleteDto>();
        public TeamCompositeDto? CompositeRating { get; set; }
    }
}
