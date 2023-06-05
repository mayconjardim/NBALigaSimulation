using System.ComponentModel.DataAnnotations;

namespace NBALigaSimulation.Shared.Models
{
    public class UserLogin
    {

        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; }

    }
}
