using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Shared.Dtos
{
    public class PlayoffsDto
    {

        public int Id { get; set; }
        public int Season { get; set; }
        public int SeriesId { get; set; }
        public bool? Complete { get; set; }
        public string teamOneAbrv { get; set; }
        public string teamOneName { get; set; }
        public string teamOneRegion { get; set; }
        public int TeamOneId { get; set; }
        public string teamTwoAbrv { get; set; }
        public string teamTwoName { get; set; }
        public string teamTwoRegion { get; set; }
        public int TeamTwoId { get; set; }
        public int WinsTeamOne { get; set; }
        public int WinsTeamTwo { get; set; }
        public List<GameCompleteDto> GameCompletes { get; set; } = new List<GameCompleteDto>();
    }
}
