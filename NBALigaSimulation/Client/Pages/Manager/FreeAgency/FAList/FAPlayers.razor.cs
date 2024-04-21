using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Manager.FreeAgency.FAList;

public partial class FAPlayers
{
    private List<PlayerCompleteDto> _faPlayers = new List<PlayerCompleteDto>();
    private List<PlayerCompleteDto> _playersRatings = new List<PlayerCompleteDto>();
    private int _season;
    private string sortedColumn = "OVR";
    private bool isAscending = false;
    private string message = string.Empty;
    private int _currentPage = 1;
    private int _pageSize = 50;
    public string _position = string.Empty;
    
    protected override async Task OnInitializedAsync()
    {
        message = "Carregando FA...";

        var result = await PlayerService.GetAllFAPlayers();
        if (!result.Success)
        {
            message = result.Message;
        }
        else
        {
            _faPlayers = result.Data;
            _playersRatings = _faPlayers;
           _season = int.Parse(await LocalStorage.GetItemAsync<string>("season"));
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
            var result = await PlayerService.GetAllFAPlayers(_currentPage, _pageSize, _season, isAscending, _position, sortedColumn);
            if (result.Success)
            {
                _faPlayers = result.Data.Players;
                _currentPage = result.Data.CurrentPage; 
            }
            StateHasChanged();
        }
        else
        {
            sortedColumn = columnName;
            isAscending = true;
            var result = await PlayerService.GetAllFAPlayers(_currentPage, _pageSize, _season, isAscending, _position, sortedColumn);
            if (result.Success)
            {
                _faPlayers = result.Data.Players;
                _currentPage = result.Data.CurrentPage; 
            }
            
            StateHasChanged();
        }
    }
    
}