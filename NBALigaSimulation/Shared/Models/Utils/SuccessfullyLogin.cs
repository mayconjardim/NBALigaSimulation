namespace NBALigaSimulation.Shared.Models.Utils;

public class SuccessfullyLogin
{

    public string Token { get; set; } = string.Empty;
    public string Team { get; set; } = string.Empty;
    public int? TeamId { get; set; }
    public bool? IsAdmin { get; set; } = false;

}