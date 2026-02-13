using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.FA;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Manager.FreeAgency.FAList;

public partial class FAPlayers
{
    private List<PlayerCompleteDto> _faPlayers;
    private List<FAOfferDto> _myOffers;
    private int _season;
    private string sortedColumn = "OVR";
    private bool isAscending = true;
    private string message = string.Empty;
    private int _currentPage = 1;
    private int _pageSize = 50;
    public string _position = string.Empty;
    private int? _removingOfferId = null;

    List<string> positions = new List<string> { "C", "FC", "PF", "F", "SF", "GF", "G", "SG", "PG", "ALL" };
    List<int> limit = new List<int> { 10, 25, 50};

    private static string FormatMoney(int value)
    {
        if (value >= 1_000_000)
        {
            double m = value / 1_000_000.0;
            return m == Math.Floor(m) ? $"${(int)m}M" : $"${m:F1}M";
        }
        if (value >= 1_000)
        {
            double k = value / 1_000.0;
            return k == Math.Floor(k) ? $"${(int)k}K" : $"${k:F1}K";
        }
        return $"${value}";
    }
    
    protected override async Task OnInitializedAsync()
    {
        message = "Carregando FA...";
        var seasonStr = await LocalStorage.GetItemAsync<string>("season");
        _season = string.IsNullOrEmpty(seasonStr) ? DateTime.Now.Year : int.Parse(seasonStr);

        var faResult = await PlayerService.GetAllFaPlayers(_currentPage, _pageSize, _season, isAscending, sortedColumn, _position);
        if (faResult.Success && faResult.Data?.Response != null)
            _faPlayers = faResult.Data.Response;
        else
            message = faResult.Message ?? "";

        var offersResult = await FAService.GetOffersByTeamId();
        _myOffers = offersResult.Success && offersResult.Data != null ? offersResult.Data : new List<FAOfferDto>();
    }

    private async Task RemoveOffer(int offerId)
    {
        if (_removingOfferId.HasValue) return;
        _removingOfferId = offerId;
        StateHasChanged();
        try
        {
            var result = await FAService.DeleteOffer(offerId);
            if (result.Success)
            {
                var refresh = await FAService.GetOffersByTeamId();
                _myOffers = (refresh.Success && refresh.Data != null) ? refresh.Data : new List<FAOfferDto>();
            }
        }
        finally
        {
            _removingOfferId = null;
            StateHasChanged();
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