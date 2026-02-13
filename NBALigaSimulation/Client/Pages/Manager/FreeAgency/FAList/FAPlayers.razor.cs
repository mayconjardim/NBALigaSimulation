using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Manager.FreeAgency.FAList;

public partial class FAPlayers
{
    private List<PlayerCompleteDto> _faPlayers;
    private List<PlayerRatingDto> _playersRatings;
    private int _season;
    private string sortedColumn = "OVR";
    private bool isAscending = true;
    private string message = string.Empty;
    private int _currentPage = 1;
    private int _pageSize = 50;
    public string _position = string.Empty;

    List<string> positions = new List<string> { "C", "FC", "PF", "F", "SF", "GF", "G", "SG", "PG", "ALL" };
    List<int> limit = new List<int> { 10, 25, 50};
    
    protected override async Task OnInitializedAsync()
    {
        message = "Carregando FA...";
            
        _season = int.Parse(await LocalStorage.GetItemAsync<string>("season"));   
            
        var result = await PlayerService.GetAllFaPlayers(_currentPage, _pageSize, _season, isAscending, sortedColumn, _position);
        
        if (!result.Success)
        {
            message = result.Message;
        }
        else
        {
            _faPlayers = result.Data.Response;
  
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
            var result = await PlayerService.GetAllFaPlayers(_currentPage, _pageSize, _season, isAscending, sortedColumn, _position);
            if (result.Success)
            {
                _faPlayers = result.Data.Response;
                _currentPage = result.Data.CurrentPage; 
            }
            StateHasChanged();
        }
        else
        {
            sortedColumn = columnName;
            isAscending = true;
            var result = await PlayerService.GetAllFaPlayers(_currentPage, _pageSize, _season, isAscending, sortedColumn, _position );
            if (result.Success)
            {
                _faPlayers = result.Data.Response;
                _currentPage = result.Data.CurrentPage; 
            }
            
            StateHasChanged();
        }
    }
    
    private async Task HandlePositionChange(ChangeEventArgs e)
    {
        _position = e.Value?.ToString();
        
        if (!string.IsNullOrEmpty(_position))
        {
            var result = await PlayerService.GetAllFaPlayers(_currentPage, _pageSize, _season, isAscending, sortedColumn, _position );
            if (result.Success)
            {
                _faPlayers = result.Data.Response;
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
                
            var result = await PlayerService.GetAllFaPlayers(_currentPage, _pageSize, _season, isAscending, sortedColumn, _position );
            if (result.Success)
            {
                _faPlayers = result.Data.Response;
                _currentPage = result.Data.CurrentPage; 
            }
        }
      
        StateHasChanged(); 
    }
}