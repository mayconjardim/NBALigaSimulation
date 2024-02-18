using System.ComponentModel.DataAnnotations;

namespace NBALigaSimulation.Shared.Models.Users
{
    public class UserRegistration
    {

        [Required]
        public string Username { get; set; }
        [Required]

        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "A senhas não são iguais!")]
        public string ConfirmPassword { get; set; }

    }
}
