using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Seasons;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.Stats.TeamStatsPage;

public partial class  TeamStatsPage
{

    private List<TeamRegularStatsDto> _teamStats;
    private List<CompleteSeasonDto> _seasonsList;
    private string _stat = null;
    private string _message = string.Empty;
    private int _season = 0;
    private string sortedColumn = "WIN%";
    private bool isAscending = false;

    protected override async Task OnInitializedAsync()
    {
        
        _season = int.Parse(await LocalStorage.GetItemAsync<string>("season"));
 
        var result = await StatsService.GetAllTeamRegularStats(_season, isAscending, _stat);
        if (result.Success)
        {
            _teamStats = result.Data;
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
            var result = await StatsService.GetAllTeamRegularStats( _season, isAscending, sortedColumn);
            if (result.Success)
            {
                _teamStats = result.Data;
            }
            StateHasChanged();
        }
        else
        {
            sortedColumn = columnName;
            isAscending = true;
            var result = await StatsService.GetAllTeamRegularStats( _season, isAscending, sortedColumn);
            if (result.Success)
            {
                _teamStats = result.Data;
            }
            StateHasChanged();
        }
       
    }
    
    private async Task HandleYearChange(ChangeEventArgs e)
    {
        string sizeString = e.Value?.ToString();
        if (int.TryParse(sizeString, out int season))
        {
            _season = season;
                
            var result = await StatsService.GetAllTeamRegularStats( _season, isAscending, sortedColumn);
            if (result.Success)
            {
                _teamStats = result.Data;
            }
        }
      
        StateHasChanged(); 
    }

    
    
}