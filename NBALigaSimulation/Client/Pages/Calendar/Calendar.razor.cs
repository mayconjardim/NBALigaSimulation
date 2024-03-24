using NBALigaSimulation.Shared.Dtos.Games;

namespace NBALigaSimulation.Client.Pages.Calendar;

public partial class Calendar
{

    private List<GameCompleteDto> _games = null;
    Dictionary<DateTime, List<GameCompleteDto>> gamesByDate = new Dictionary<DateTime, List<GameCompleteDto>>();
        
    string[] headings = { "DATE", "HOME", "AWAY", "W-L", "RESULT" };
    
    protected override async Task OnInitializedAsync()
    {
        var result = await GameService.GetAllGames();
    
        if (result.Success)
        {
            _games = result.Data;
        
            var uniqueDates = _games.Select(game => game.GameDate.Date).Distinct().OrderBy(date => date).ToList();
            gamesByDate = new Dictionary<DateTime, List<GameCompleteDto>>();
        
            foreach (var date in uniqueDates)
            {
                var gamesOnDate = _games.Where(game => game.GameDate.Date == date).ToList();
                gamesByDate.Add(date, gamesOnDate);
            }
        
        }
    }

}