using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Pages.Stats.PlayerStatsPage;

public partial class PlayerStatsPage
{
    private PlayerStatsResponse _statsResponse;
    private List<PlayerRegularStatsDto> _playerStats;
    private int _currentPage = 1;
    private int _pageSize = 50;
    private string _stat = null;
    private string _message = string.Empty;
    private int _season = 2007;
    private string sortedColumn = "PPG";
    private bool isAscending = false;
    public string position = string.Empty;

    List<string> positions = new List<string> { "C", "FC", "PF", "F", "SF", "GF", "G", "SG", "PG" };

    protected override async Task OnInitializedAsync()
    {
        var result = await StatsService.GetAllPlayerRegularStats(_currentPage, _pageSize, _season, isAscending, position);

        if (result.Success)
        {
            _statsResponse = result.Data;
            _playerStats = _statsResponse.Stats;
            _currentPage = _statsResponse.CurrentPage;
        }
        else
        {
            _message = result.Message;
        }
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
            await StatsService.GetAllPlayerRegularStats(_currentPage, _pageSize, _season, isAscending, position);
        }
        else
        {
            sortedColumn = columnName;
            isAscending = true;
            await StatsService.GetAllPlayerRegularStats(_currentPage, _pageSize, _season, isAscending, position);
        }
        
        StateHasChanged();
    }

    private async Task HandleCategoryChange(ChangeEventArgs e)
    {
        position = e.Value?.ToString();
        
        if (!string.IsNullOrEmpty(position))
        {
          var result = await StatsService.GetAllPlayerRegularStats(_currentPage, _pageSize, _season, isAscending, position);
          if (result.Success)
          {
              _playerStats = result.Data.Stats;
              _currentPage = result.Data.CurrentPage; 
          }
        }

        StateHasChanged(); 
    }
    


}



    
