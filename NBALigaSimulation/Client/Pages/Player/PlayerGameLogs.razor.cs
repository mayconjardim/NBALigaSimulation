using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace NBALigaSimulation.Client.Pages.Player
{
    partial class PlayerGameLogs
    {

        [Parameter]
        public PlayerCompleteDto? player { get; set; }

        private List<PlayerGameStatsDto> logs = null;

        string[] headings = { "DATE", "OPP", "MIN", "FG", "3PT", "FT", "OFF", "REB", "AST", "TO", "STL", "BLK", "PF", "PTS" };

        protected override void OnInitialized()
        {
            logs = player.Stats.ToList();
        }

        public string Format(double numero)
        {
            string numeroFormatado = numero.ToString("0.0", CultureInfo.InvariantCulture);
            return numeroFormatado;
        }
    }
}
