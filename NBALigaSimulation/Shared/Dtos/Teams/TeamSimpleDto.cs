namespace NBALigaSimulation.Shared.Dtos.Teams
{
    public class TeamSimpleDto
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Abrv { get; set; } = string.Empty;
        public string Conference { get; set; } = string.Empty;

    }
}
