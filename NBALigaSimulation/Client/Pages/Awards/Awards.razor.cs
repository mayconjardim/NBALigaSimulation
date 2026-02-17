using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Client.Services.AwardsService;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Awards;

public partial class Awards
{
    [Inject]
    private IAwardsService AwardsService { get; set; }

    [Inject]
    private ILocalStorageService LocalStorage { get; set; }

    private List<PlayerAwardsDto> _awards = new();
    private Dictionary<int, List<PlayerAwardsDto>> _awardsBySeason = new();
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadAwards();
    }

    private async Task LoadAwards()
    {
        _isLoading = true;
        try
        {
            var response = await AwardsService.GetAllAwards();
            if (response.Success && response.Data != null)
            {
                _awards = response.Data;
                
                _awardsBySeason = _awards
                    .GroupBy(a => a.Season)
                    .OrderByDescending(g => g.Key)
                    .ToDictionary(g => g.Key, g => g.OrderBy(a => GetAwardOrder(a.Award)).ToList());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar awards: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private int GetAwardOrder(string award)
    {
        return award switch
        {
            "MVP" => 1,
            "DPOY" => 2,
            "Sixth Man of the Year" => 3,
            "NBA Finals MVP" => 4,
            _ => 99
        };
    }

    private string FormatStat(double value)
    {
        return value.ToString("0.0");
    }
}
