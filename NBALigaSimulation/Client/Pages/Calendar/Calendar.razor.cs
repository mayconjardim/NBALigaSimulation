using NBALigaSimulation.Shared.Dtos.Games;

namespace NBALigaSimulation.Client.Pages.Calendar;

public partial class Calendar
{
    private List<GameCompleteDto> _games;
    private Dictionary<string, List<GameCompleteDto>> gamesByRound = new();
    private List<string> gameRounds = new();

    private string selectedRound = "";
    private int visibleRoundsStartIndex = 0;
    private const int NumberOfVisibleRounds = 7;

    protected override async Task OnInitializedAsync()
    {
        var result = await GameService.GetAllGames();

        if (result.Success && result.Data.Any())
        {
            _games = result.Data;
            gameRounds = _games
                .Select(g => g.Week ?? "")
                .Where(w => !string.IsNullOrEmpty(w))
                .Distinct()
                .OrderBy(w => int.TryParse(w, out var n) ? n : 0)
                .ToList();

            gamesByRound = gameRounds.ToDictionary(
                r => r,
                r => _games.Where(g => g.Week == r).OrderBy(g => g.GameDate).ToList()
            );

            var firstUnplayedRound = gameRounds
                .FirstOrDefault(r => gamesByRound[r].Any(g => g.HomeTeamScore <= 0));

            if (!string.IsNullOrEmpty(firstUnplayedRound))
            {
                selectedRound = firstUnplayedRound;
            }
            else
            {
                selectedRound = gameRounds.LastOrDefault() ?? "";
            }

            var selectedIndex = gameRounds.IndexOf(selectedRound);
            if (selectedIndex != -1)
            {
                var potentialStartIndex = selectedIndex - (NumberOfVisibleRounds / 2);
                potentialStartIndex = Math.Max(0, potentialStartIndex);
                potentialStartIndex = Math.Min(potentialStartIndex, Math.Max(0, gameRounds.Count - NumberOfVisibleRounds));
                visibleRoundsStartIndex = potentialStartIndex;
            }
        }
    }

    private void SelectRound(string round)
    {
        selectedRound = round;
    }

    private void ChangeRoundView(int direction)
    {
        var newStartIndex = visibleRoundsStartIndex + (direction * NumberOfVisibleRounds);
        visibleRoundsStartIndex = Math.Max(0, Math.Min(newStartIndex, gameRounds.Count - NumberOfVisibleRounds));
    }

    private IEnumerable<string> GetDisplayedRounds()
    {
        if (gameRounds == null) return Enumerable.Empty<string>();
        return gameRounds.Skip(visibleRoundsStartIndex).Take(NumberOfVisibleRounds);
    }
}