using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;
using pax.BlazorChartJs;

namespace NBALigaSimulation.Client.Pages.Players.PlayerPage;

public partial class PlayerRating
{
    
    [Parameter]
    public PlayerCompleteDto _player { get; set; }
    private PlayerRatingDto _rating;
    private List<string> _badgesSkills = new List<string> { "Po", "3", "Ps", "B" };
    private List<string> _badgesPhysical = new List<string> { "Dp", "Di", "R", "A" };
    
    ChartComponent _chartComponent;
    ChartJsConfig _chartJsConfig = null!;

    ChartComponent _chartComponent2;
    ChartJsConfig _chartJsConfig2 = null!;

    private int _season = 0;
    private int _age = 0;
    
     protected override void OnInitialized()
     {

         if (_player != null)
         {
             _rating = _player.Ratings.LastOrDefault();
             _season = _rating.Season;
             _age = (_season - _player.Born.Year);
         }
         
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
                        BackgroundColor = "#391463",
                        Color = "#ffffff",
                        BorderRadius = 10,
                        Padding = new Padding()
                        {
                            Left = 2,
                            Right = 2,
                            Top = 2,
                            Bottom = 2,
                        },
                        Font = new Font()
                        {
                            Size = 10,
                            Weight = "bold"
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
                            Label = "",
                            Data = new List<object>() { _rating.Ins, _rating.Dnk, _rating.Ft, _rating.Fg, _rating.Tp, _rating.Drb, _rating.Pss },
                            BackgroundColor = "rgba(57, 20, 99, 0.83)",
                            BorderColor = "#391463",
                            BorderWidth = 3
                        }
                    }
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
                            Label = "",
                            Data = new List<object>() {  _rating.Oiq, _rating.Diq, _rating.Reb, _rating.Hgt, _rating.Stre, _rating.Spd, _rating.Jmp, _rating.Endu },
                            BackgroundColor = "rgba(57, 20, 99, 0.83)",
                            BorderColor = "#391463",
                            BorderWidth = 3
                        }
                    }
                },

                Options = Options

            };

            base.OnInitialized();

     }
     
     private string GetBadgeName(string badge)
     {
         switch (badge)
         {
             case "Po":
                 return "Post Scorer";
             case "3":
                 return "3-Point Shooter";
             case "Ps":
                 return "Passer";
             case "B":
                 return "Ball-Handler";
             case "Dp":
                 return "Perimeter Defender";
             case "Di":
                 return "Interior Defender";
             case "R":
                 return "Rebounder";
             default:
                 return "Athlete";
         }
     }
    
}