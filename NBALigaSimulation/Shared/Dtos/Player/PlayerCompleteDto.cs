namespace NBALigaSimulation.Shared.Dtos
{
    public class PlayerCompleteDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
        public int BornYear { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public int TeamId { get; set; }
        public List<PlayerRatingDto> Ratings { get; set; } = new List<PlayerRatingDto>();
        public string GetFullName()
        {
            return FirstName + " " + LastName;
        }
    }
}
