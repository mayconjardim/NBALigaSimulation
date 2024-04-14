using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Seasons;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Pages.Stats.PlayerStatsPage;

public partial class PlayerStatsPage
{
    private PageableStatsResponse<PlayerRegularStatsDto> _statsResponse;
    private List<PlayerRegularStatsDto> _playerStats;
    private List<CompleteSeasonDto> _seasonsList;
    private int _currentPage = 1;
    private int _pageSize = 50;
    private string _stat = null;
    private string _message = string.Empty;
    private int _season = 0;
    private string sortedColumn = "PPG";
    private bool isAscending = false;
    public string position = string.Empty;

    List<string> positions = new List<string> { "C", "FC", "PF", "F", "SF", "GF", "G", "SG", "PG" };
    List<int> limit = new List<int> { 10, 25, 50};


    protected override async Task OnInitializedAsync()
    {
        _season = int.Parse(await LocalStorage.GetItemAsync<string>("season"));
 
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

        var response = await SeasonService.GetALlSeason();
        if (response.Success)
        {
            _seasonsList = response.Data;
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
            var result = await StatsService.GetAllPlayerRegularStats(_currentPage, _pageSize, _season, isAscending, position, sortedColumn);
            if (result.Success)
            {
                _playerStats = result.Data.Stats;
                _currentPage = result.Data.CurrentPage; 
            }
            StateHasChanged();
        }
        else
        {
            sortedColumn = columnName;
            isAscending = true;
            var result = await StatsService.GetAllPlayerRegularStats(_currentPage, _pageSize, _season, isAscending, position, sortedColumn);
            if (result.Success)
            {
                _playerStats = result.Data.Stats;
                _currentPage = result.Data.CurrentPage; 
            }
            StateHasChanged();
        }
        
       
    }

    private async Task HandlePositionChange(ChangeEventArgs e)
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
        
    private async Task HandleSizeChange(ChangeEventArgs e)
    {
            string sizeString = e.Value?.ToString();
            if (int.TryParse(sizeString, out int newPageSize))
            {
                _pageSize = newPageSize;
                
                var result = await StatsService.GetAllPlayerRegularStats(_currentPage, _pageSize, _season, isAscending, position);
                if (result.Success)
                {
                    _playerStats = result.Data.Stats;
                    _currentPage = result.Data.CurrentPage; 
                }
            }
      
          StateHasChanged(); 
    }
    
    private async Task HandleYearChange(ChangeEventArgs e)
    {
        string sizeString = e.Value?.ToString();
        if (int.TryParse(sizeString, out int season))
        {
            _season = season;
                
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



    
