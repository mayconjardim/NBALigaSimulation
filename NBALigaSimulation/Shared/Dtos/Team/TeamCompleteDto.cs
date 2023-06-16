
using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Shared.Dtos
{
    public class TeamCompleteDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Abrv { get; set; } = string.Empty;
        public string Conference { get; set; } = string.Empty;

        public List<PlayerCompleteDto> Players { get; set; } = new List<PlayerCompleteDto>();
        public TeamGameplanDto Gameplan { get; set; }
    }
}
