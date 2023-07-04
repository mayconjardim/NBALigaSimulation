using Microsoft.AspNetCore.Components;

namespace NBALigaSimulation.Client.Pages.Player
{
    partial class PlayerStats
    {

        [Parameter]
        public PlayerCompleteDto? player { get; set; }

        string[] headings = { "YEAR", "TEAM", "GP", "GS" ,"MIN", "M" , "A", "%", "M", "A" , "%", "M" , "A", "%" ,
            "OFF", "DEF" , "TOT", "AST", "TO", "STL",  "BLK", "PF" , "PTS", "PER"};


    }
}
