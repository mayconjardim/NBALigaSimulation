using NBALigaSimulation.Shared.Dtos.Games;
using NBALigaSimulation.Shared.Dtos.Teams;
using System.Globalization;

namespace NBALigaSimulation.Client.Shared
{
    public partial class Simup
    {

        private List<TeamSimpleDto> teams = new List<TeamSimpleDto>();

        private TeamCompleteDto? homeTeam;
        private TeamCompleteDto? awayTeam;

        private GameCompleteDto games;
        private GameCompleteDto newGame;

        private int homeTeamId = 99;
        private int awayTeamId = 99;

        private string message = string.Empty;

        string[] headings = { "NAME", "MIN", "FG", "3PT", "FT", "OFF", "REB", "AST", "TO", "STL", "BLK", "PF", "PTS" };

        protected override async Task OnInitializedAsync()
        {
            message = "Carregando Times...";

            var result = await TeamService.GetAllTeams();
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                teams = result.Data.ToList();
            }
        }

        private async Task OnAwayTeamSelected(int value)
        {
            message = "Carregando Time...";
            var result = await TeamService.GetTeamById(value);
            homeTeamId = value;
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                awayTeam = result.Data;
            }
        }

        private async Task OnHomeTeamSelected(int value)
        {
            message = "Carregando Time...";
            var result = await TeamService.GetTeamById(value);
            awayTeamId = value;
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                homeTeam = result.Data;
            }
        }

        private async Task CreateGame()
        {

            var game = new CreateGameDto
            {
                HomeTeamId = homeTeamId,
                AwayTeamId = awayTeamId
            };

            games = await GameService.CreateGame(game);

            if (games != null)
            {
                int gameId = games.Id;
                var result = await GameService.UpdateGame(gameId);
            }

            if (games != null)
            {
                int gameId = games.Id;
                var result = await GameService.GetGameById(gameId);
                newGame = result.Data;
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
