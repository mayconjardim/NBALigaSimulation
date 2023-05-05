using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using pax.BlazorChartJs;

namespace NBALigaSimulation.Client.Pages.Player
{
    partial class PlayerRatings
    {
        [Parameter]
        public PlayerCompleteDto? player { get; set; }

        private PlayerRatingDto? rating => player.Ratings.LastOrDefault();

        ChartComponent? chartComponent;
        ChartJsConfig chartJsConfig = null!;

        ChartComponent? chartComponent2;
        ChartJsConfig chartJsConfig2 = null!;

        protected override void OnInitialized()
        {

            var Options = new ChartJsOptions()
            {
                Responsive = true,
                MaintainAspectRatio = true,
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
                            Left = 5,
                            Right = 5,
                        },
                        Font = new Font()
                        {
                            Size = 10,
                            Weight = "bold"
                        },

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
                            // Color = "yellow"
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
                        "Inside2", "La232yups", "Free thr33ows", "Two poin3ters", "Three pointers", "Dribbling", "Passing"
                    },
                    Datasets = new List<ChartJsDataset>()
                    {
                        new RadarDataset()
                        {
                            Label = "",
                            Data = new List<object>() { rating.Ins, rating.Dnk, rating.Ft, rating.Tp, rating.Fg, rating.Drb, rating.Pss },
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
                        "Inside", "Layups", "Free throws", "Two pointers", "Three pointers", "Dribbling", "Passing"
                    },
                    Datasets = new List<ChartJsDataset>()
                    {
                        new RadarDataset()
                        {
                            Label = "",
                            Data = new List<object>() { rating.Ins, rating.Dnk, rating.Ft, rating.Tp, rating.Fg, rating.Drb, rating.Pss },
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
