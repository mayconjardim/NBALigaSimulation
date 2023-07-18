using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace NBALigaSimulation.Client.Pages.Game
{
    partial class Game
    {
        string[] headings = { "NAME", "MIN", "FG", "3PT", "FT", "OFF", "REB", "AST", "TO", "STL", "BLK", "PF", "PTS" };

        private GameCompleteDto? game = null;
        private string message = string.Empty;
        private TeamGameStatsDto? homeTeamStat = null;
        private TeamGameStatsDto? awayTeamStat = null;


        [Parameter]
        public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            message = "Carregando jogo...";

            var result = await GameService.GetGameById(Id);
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                game = result.Data;

                foreach (var gameStat in game.TeamGameStats)
                {
                    if (game.HomeTeamId == gameStat.TeamId)
                    {
                        homeTeamStat = gameStat;
                    }
                    else if (game.AwayTeamId == gameStat.TeamId)
                    {
                        awayTeamStat = gameStat;
                    }
                }

            }
        }

        public string NomeAbvr(string nomeCompleto)
        {
            string[] partesNome = nomeCompleto.Split(' ');

            if (partesNome.Length < 2)
            {
                return nomeCompleto;
            }

            string primeiroNome = partesNome[0];
            string sobrenome = partesNome[partesNome.Length - 1];

            string primeiraLetra = primeiroNome.Substring(0, 1);

            return $"{primeiraLetra}. {sobrenome}";
        }

        public string Format(double numero)
        {
            string numeroFormatado = numero.ToString("0.0", CultureInfo.InvariantCulture);
            return numeroFormatado;
        }


    }
}
