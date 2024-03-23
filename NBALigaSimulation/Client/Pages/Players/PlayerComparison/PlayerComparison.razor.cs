using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;
using pax.BlazorChartJs;

namespace NBALigaSimulation.Client.Pages.Players.PlayerComparison;

public partial class PlayerComparison
{
    
    private PlayerCompleteDto _player1;
    private PlayerCompleteDto _player2;
    private PlayerRegularStatsDto _player1Stats;
    private PlayerRegularStatsDto _player2Stats;
    private PlayerRatingDto _playerRating1;
    private PlayerRatingDto _playerRating2;
    private List<PlayerSimpleDto> suggestions = new List<PlayerSimpleDto>();
    private string _playerName = string.Empty;
    private string _message = string.Empty;
    private string searchText = string.Empty;
    private string searchText2 = string.Empty;
    protected ElementReference searchInput;
    
    ChartComponent _chartComponent;
    ChartJsConfig _chartJsConfig = null!;

    ChartComponent _chartComponent2;
    ChartJsConfig _chartJsConfig2 = null!;
 
    public async Task HandleSearchOne(KeyboardEventArgs args)
    {
        if (args.Key == null || args.Key.Equals("Enter"))
        {
            if (!string.IsNullOrEmpty(searchText) && int.TryParse(searchText, out int selectedPlayerId))
            {
                var response = await PlayerService.GetPlayerById(selectedPlayerId);
                if (response.Success)
                {
                    _player1 = response.Data;
                    _player1Stats = _player1.RegularStats.LastOrDefault();
                    _playerRating1 = _player1.Ratings.LastOrDefault();
                }
            }
        }
        else if (searchText.Length > 1)
        {

            ServiceResponse<List<PlayerSimpleDto>> response = await PlayerService.GetPlayersSearchSuggestions(searchText);
            if (response.Success) 
            {
                suggestions = response.Data; 
            }
        }
    }
    
    public async Task HandleSearchTwo(KeyboardEventArgs args)
    {
        if (args.Key == null || args.Key.Equals("Enter"))
        {
            if (!string.IsNullOrEmpty(searchText2) && int.TryParse(searchText2, out int selectedPlayerId))
            {
                var response = await PlayerService.GetPlayerById(selectedPlayerId);
                if (response.Success)
                {
                    _player2 = response.Data;
                    _player2Stats = _player2.RegularStats.LastOrDefault();
                    _playerRating2 = _player2.Ratings.LastOrDefault(); 
                    await OnAfterRenderAsync(true);
                }
            }
        }
        else if (searchText2.Length > 1)
        {

            ServiceResponse<List<PlayerSimpleDto>> response = await PlayerService.GetPlayersSearchSuggestions(searchText2);
            if (response.Success) 
            {
                suggestions = response.Data; 
            }
        }
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
     {
  
         if (_player1 != null && _playerRating1 != null && _player2 != null && _playerRating2 != null)
         {
             var Options = new ChartJsOptions()
             {
                 
                 Responsive = true,
                 MaintainAspectRatio = false,
                 Animation = false,
                 Plugins = new Plugins()
                 {
                     Legend = new Legend()
                     {
                         Display = false
                     },
                     Title = new Title()
                     {
                         Display = false,
                     },

                     Datalabels = new DataLabelsConfig()
                     {

                         Display = true,
                         Color = "#fff",
                         Font = new Font()
                         {
                             Size = 14,
                             Weight = "bolder"
                         },
                     },
                     Tooltip = new Tooltip()
                     {
                         Enabled = false
                     }
                 },
                 Scales = new ChartJsOptionsScales()
                 {
                     R = new LinearRadialAxis()
                     {

                         Ticks = new LinearAxisTick
                         {
                             Display = false
                         },

                         Grid = new ChartJsGrid()
                         {
                         },
                         SuggestedMin = 0,
                         SuggestedMax = 100
                     }
                 }
             };

             _chartJsConfig = new()
             {
                 Type = ChartType.radar,
                 Data = new ChartJsData()
                 {
                     Labels = new List<string>()
                     {
                         "INS", "DNK/LAY", "FT", "2P", "3P", "DRL", "PAS"
                     },
                     Datasets = new List<ChartJsDataset>()
                     {
                         new RadarDataset()
                         {
                             Label = _player1.Name,
                             Data = new List<object>()
                             {
                                 _playerRating1.Ins, _playerRating1.Dnk, _playerRating1.Ft, _playerRating1.Fg, 
                                 _playerRating1.Tp, _playerRating1.Drb, _playerRating1.Pss
                             },
                             BorderColor = "#391463",
                             BorderWidth = 3
                         },
                         
                         new RadarDataset()
                         {
                             Label = _player2.Name,
                             Data = new List<object>()
                             {
                                 _playerRating2.Ins, _playerRating2.Dnk, _playerRating2.Ft, _playerRating2.Fg, 
                                 _playerRating2.Tp, _playerRating2.Drb, _playerRating2.Pss
                             },
                             BorderColor = "#FF6384",
                             BorderWidth = 3
                         },
                         
                     },
                     
                     
                 },
                 Options = Options

             };

             _chartJsConfig2 = new()
             {
                 Type = ChartType.radar,
                 Data = new ChartJsData()
                 {
                     Labels = new List<string>()
                     {
                         "OIQ", "DIQ", "REB", "HGT", "STR", "SPD", "JMP", "END"
                     },
                     Datasets = new List<ChartJsDataset>()
                     {
                         new RadarDataset()
                         {
                             Label = _player1.Name,
                             Data = new List<object>()
                             {
                                 _playerRating1.Oiq, _playerRating1.Diq, _playerRating1.Reb, _playerRating1.Hgt, _playerRating1.Stre, 
                                 _playerRating1.Spd,_playerRating1.Jmp, _playerRating1.Endu
                             },
                             BorderColor = "#391463",
                             BorderWidth = 3
                         },
                         new RadarDataset()
                         {
                             Label = _player2.Name,
                             Data = new List<object>()
                             {
                                 _playerRating2.Oiq, _playerRating2.Diq, _playerRating2.Reb, _playerRating2.Hgt, _playerRating2.Stre, 
                                 _playerRating2.Spd,_playerRating2.Jmp, _playerRating2.Endu
                             },
                             BorderColor = "#FF6384",
                             BorderWidth = 3
                         }
                         
                         
                     }
                 },

                 Options = Options

             };
            
             await base.OnAfterRenderAsync(firstRender); 
         }
     }

    public async Task UpdateChartAsync()
    {
        OnAfterRenderAsync(true);
        StateHasChanged();
    }
    
    public async void attChart()
    {
        StateHasChanged();
        await OnAfterRenderAsync(true);
    }
}