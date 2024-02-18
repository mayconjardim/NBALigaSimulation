using System.ComponentModel.DataAnnotations;

namespace NBALigaSimulation.Shared.Models.Users
{
    public class UserChangePassword
    {

        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
        [Compare("Password", ErrorMessage = "A senhas não são iguais!")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
