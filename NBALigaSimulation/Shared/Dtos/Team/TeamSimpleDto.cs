using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Shared.Dtos
{
    public class TeamCompleteDto
    {

        public string Name { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Abrv { get; set; } = string.Empty;
        public List<PlayerSimpleDto> Players { get; set; } = new List<PlayerSimpleDto>();

    }
}
