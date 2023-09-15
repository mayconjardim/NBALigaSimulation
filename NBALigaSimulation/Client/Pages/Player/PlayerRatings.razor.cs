using Microsoft.AspNetCore.Components;
using pax.BlazorChartJs;


namespace NBALigaSimulation.Client.Pages.Player
{
    partial class PlayerRatings
    {
        [Parameter]
        public PlayerCompleteDto? player { get; set; }

        private PlayerRatingDto? rating => player.Ratings.LastOrDefault();

        private List<string> badgesSkills = new List<string> { "Po", "3", "Ps", "B" };
        private List<string> badgesPhysical = new List<string> { "Dp", "Di", "R", "A" };

        public int season = 0;

        public string GetBadgeName(string badge)
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

        ChartComponent? chartComponent;
        ChartJsConfig chartJsConfig = null!;

        ChartComponent? chartComponent2;
        ChartJsConfig chartJsConfig2 = null!;

        protected override async Task OnInitializedAsync()
        {
            season = int.Parse(await LocalStorage.GetItemAsync<string>("season"));
        }

        protected override void OnInitialized()
        {

            var Options = new ChartJsOptions()
            {
                Responsive = true,
                MaintainAspectRatio = false,
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
                        BackgroundColor = "#03234C",
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

                        AngelLines = new AngelLines()
                        {
                            Display = false
                        },
                        Grid = new ChartJsGrid()
                        {
                        },
                        SuggestedMin = 0,
                        SuggestedMax = 0
                    }
                }
            };

            chartJsConfig2 = new()
            {
                Type = ChartType.radar,
                Data = new ChartJsData()
                {
                    Labels = new List<string>()
                    {
                      "DIQ", "REB", "OIQ", "HGT", "STR", "SPD", "JMP", "END"
                    },
                    Datasets = new List<ChartJsDataset>()
                    {
                        new RadarDataset()
                        {
                            Label = "",
                            Data = new List<object>() { rating.Diq, rating.Reb, rating.Oiq, rating.Hgt, rating.Stre, rating.Spd, rating.Jmp, rating.Endu },
                            BackgroundColor = "rgba(3, 35, 76, 0.83)",
                            BorderColor = "#03234C",
                            BorderWidth = 3
                        }
                    }
                },

                Options = Options

            };

            chartJsConfig = new()
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
                            Data = new List<object>() { rating.Ins, rating.Dnk, rating.Ft, rating.Fg, rating.Tp, rating.Drb, rating.Pss },
                            BackgroundColor = "rgba(3, 35, 76, 0.83)",
                            BorderColor = "#03234C",
                            BorderWidth = 3
                        }
                    }
                },

                Options = Options

            };

            base.OnInitialized();

        }

    }
}
