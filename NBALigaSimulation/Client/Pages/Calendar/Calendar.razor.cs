using NBALigaSimulation.Shared.Dtos.Games;
using System.Globalization;

namespace NBALigaSimulation.Client.Pages.Calendar;

public partial class Calendar
{

    private List<GameCompleteDto> _games;
    private Dictionary<DateTime, List<GameCompleteDto>> gamesByDate = new();
    private List<DateTime> gameDates = new();
    
    private DateTime selectedDate;
    private int visibleDatesStartIndex = 0;
    private const int NumberOfVisibleDates = 7;

    protected override async Task OnInitializedAsync()
    {
        var result = await GameService.GetAllGames();
    
        if (result.Success && result.Data.Any())
        {
            _games = result.Data;
            gameDates = _games.Select(g => g.GameDate.Date).Distinct().OrderBy(d => d).ToList();
            gamesByDate = gameDates.ToDictionary(d => d, d => _games.Where(g => g.GameDate.Date == d).ToList());

            var firstUnplayedGameDate = gameDates
                .FirstOrDefault(d => gamesByDate[d].Any(g => g.HomeTeamScore <= 0));

            if (firstUnplayedGameDate != default)
            {
                selectedDate = firstUnplayedGameDate;
            }
            else
            {
                selectedDate = gameDates.LastOrDefault();
            }

            var selectedIndex = gameDates.IndexOf(selectedDate);
            if (selectedIndex != -1)
            {
                var potentialStartIndex = selectedIndex - (NumberOfVisibleDates / 2);
                potentialStartIndex = Math.Max(0, potentialStartIndex);
                potentialStartIndex = Math.Min(potentialStartIndex, Math.Max(0, gameDates.Count - NumberOfVisibleDates));
                visibleDatesStartIndex = potentialStartIndex;
            }
        }
    }

    private void SelectDate(DateTime date)
    {
        selectedDate = date;
    }

    private void ChangeDateView(int direction)
    {
        var newStartIndex = visibleDatesStartIndex + (direction * NumberOfVisibleDates);
        visibleDatesStartIndex = Math.Max(0, Math.Min(newStartIndex, gameDates.Count - NumberOfVisibleDates));
    }

    private IEnumerable<DateTime> GetDisplayedGameDates()
    {
        if (gameDates == null) return Enumerable.Empty<DateTime>();
        return gameDates.Skip(visibleDatesStartIndex).Take(NumberOfVisibleDates);
    }
    
    private CultureInfo GetPortugueseCulture()
    {
        return new CultureInfo("pt-BR");
    }

}