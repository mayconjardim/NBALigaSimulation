using NBALigaSimulation.Shared.Dtos.Teams;
using Microsoft.AspNetCore.Components;

namespace NBALigaSimulation.Client.Pages.League;

public partial class DraftPicks
{
    private List<TeamDraftPickDto> _allPicks = new();
    private List<TeamDraftPickDto> _filteredPicks = new();
    
    private List<int> _availableYears = new();
    private List<TeamSimpleDto> _availableTeams = new();

    private int _selectedYear;
    private int _selectedRound = 1;
    private int? _selectedTeamId = null;

    public string message { get; set; }

    protected override async Task OnInitializedAsync()
    {
        message = "Carregando Draft Picks...";
        var result = await LeagueService.GetAllDraftPicks();

        if (result.Success && result.Data.Any())
        {
            _allPicks = result.Data;

            _availableYears = _allPicks.Select(p => p.Year).Distinct().OrderBy(y => y).ToList();
            _availableTeams = _allPicks
                .Select(p => new TeamSimpleDto { Id = p.TeamId, Name = p.TeamName })
                .GroupBy(t => t.Id)
                .Select(g => g.First())
                .OrderBy(t => t.Name)
                .ToList();
            
            if (_availableYears.Any())
            {
                _selectedYear = _availableYears.First();
            }
            
            ApplyFilters();
        }
        else if (!result.Success)
        {
            message = result.Message;
        }
    }

    private void ApplyFilters()
    {
        _filteredPicks = _allPicks
            .Where(p => p.Year == _selectedYear)
            .Where(p => p.Round == _selectedRound)
            .Where(p => !_selectedTeamId.HasValue || p.TeamId == _selectedTeamId.Value)
            .ToList();
    }

    private void SelectYear(int year)
    {
        _selectedYear = year;
        ApplyFilters();
    }

    private void SelectRound(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var round))
        {
            _selectedRound = round;
            ApplyFilters();
        }
    }

    private void SelectTeam(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var teamId))
        {
            _selectedTeamId = teamId == 0 ? null : teamId;
            ApplyFilters();
        }
    }
}