using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Pages.Stats.PlayerStatsPage;

public partial class PlayerStatsPage
{
    private PlayerStatsResponse _statsResponse = new PlayerStatsResponse();
    private List<PlayerRegularStatsDto> _playerStats;
    private int _currentPage = 1;
    private int _pageSize = 50;
    private string _stat;
    private string _message = string.Empty;
    private int _season = 2007;
    private string sortedColumn = "PPG";
    private bool isAscending = false;
    private string position = string.Empty;

    List<string> positions = new List<string> { "C", "FC", "PF", "F", "SF", "GF", "G", "SG", "PG" };

    protected override async Task OnInitializedAsync()
    {
        await GetAllPlayerRegularStatsOrdered();
    }
    
    private async Task GetAllPlayerRegularStatsOrdered()
    {
        var result = await StatsService.GetAllPlayerRegularStats(_currentPage, _pageSize, _season, isAscending, sortedColumn, position);

        if (result.Success)
        {
            _statsResponse = result.Data ?? new PlayerStatsResponse();
            _playerStats = _statsResponse.Stats ?? new List<PlayerRegularStatsDto>();
            _currentPage = _statsResponse.CurrentPage;
        }
        else
        {
            _message = result.Message;
        }

        StateHasChanged();
    }
    
    private string GetSortIcon(string columnName)
    {
        if (columnName == sortedColumn)
        {
            return isAscending ? "bi-sort-up" : "bi-sort-down";
        }
        return string.Empty;
    }

    private async void SortTable(string columnName)
    {
        if (columnName == sortedColumn)
        {
            isAscending = !isAscending;
            await GetAllPlayerRegularStatsOrdered();
        }
        else
        {
            sortedColumn = columnName;
            isAscending = true;
            await GetAllPlayerRegularStatsOrdered();
        }
        StateHasChanged(); 
    }

    private async void FilterByPosition(ChangeEventArgs e)
    {
        position = e.Value.ToString();
        await GetAllPlayerRegularStatsOrdered();
        StateHasChanged();

    }


}



    
