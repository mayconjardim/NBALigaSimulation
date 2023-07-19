namespace NBALigaSimulation.Shared.Dtos
{
    public class PlayerSimpleDto
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
        public int BornYear { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public int TeamId { get; set; }

    }
}
