using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Client.Services.GameService;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Players.PlayerPage;

public partial class CareerBests
{
    [Parameter]
    public PlayerCompleteDto _player { get; set; }

    private int MaxPoints { get; set; }
    private int MaxAssists { get; set; }
    private int MaxRebounds { get; set; }
    private int MaxBlocks { get; set; }
    private int MaxSteals { get; set; }
    private int DoubleDoubles { get; set; }
    private int TripleDoubles { get; set; }
    private int PlayerOfTheGame { get; set; }
    private int FinalsMVP { get; set; }

    protected override void OnInitialized()
    {
        if (_player?.Stats != null && _player.Stats.Any())
        {
            CalculateCareerBests();
        }
        
        // Usar dados salvos do banco
        if (_player?.AwardCounts != null)
        {
            PlayerOfTheGame = _player.AwardCounts.PlayerOfTheGame;
        }

        // Contar Finals MVP dos PlayerAwards
        if (_player?.PlayerAwards != null)
        {
            FinalsMVP = _player.PlayerAwards.Count(a => a.Award == "NBA Finals MVP");
        }
    }

    private void CalculateCareerBests()
    {
        var stats = _player.Stats;

        // Calcular máximos
        MaxPoints = stats.Max(s => s.Pts);
        MaxAssists = stats.Max(s => s.Ast);
        MaxRebounds = stats.Max(s => s.Trb);
        MaxBlocks = stats.Max(s => s.Blk);
        MaxSteals = stats.Max(s => s.Stl);

        // Calcular Double Doubles e Triple Doubles
        foreach (var game in stats)
        {
            int categoriesWith10Plus = 0;

            if (game.Pts >= 10) categoriesWith10Plus++;
            if (game.Ast >= 10) categoriesWith10Plus++;
            if (game.Trb >= 10) categoriesWith10Plus++;
            if (game.Blk >= 10) categoriesWith10Plus++;
            if (game.Stl >= 10) categoriesWith10Plus++;

            if (categoriesWith10Plus >= 2)
            {
                DoubleDoubles++;
            }

            if (categoriesWith10Plus >= 3)
            {
                TripleDoubles++;
            }
        }
    }

}
