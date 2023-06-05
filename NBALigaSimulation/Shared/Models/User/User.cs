namespace NBALigaSimulation.Shared.Models
{
    public class User
    {

        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime DataCreated { get; set; }
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
    }
}
