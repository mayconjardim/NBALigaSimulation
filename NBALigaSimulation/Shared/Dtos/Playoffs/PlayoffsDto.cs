namespace NBALigaSimulation.Shared.Dtos
{
    public class PlayoffsDto
    {

        public int Id { get; set; }
        public int Season { get; set; }
        public int SeriesId { get; set; }
        public bool? Complete { get; set; }
        public int TeamOneId { get; set; }
        public int TeamTwoId { get; set; }
        public int WinsTeamOne { get; set; }
        public int WinsTeamTwo { get; set; }

    }
}
