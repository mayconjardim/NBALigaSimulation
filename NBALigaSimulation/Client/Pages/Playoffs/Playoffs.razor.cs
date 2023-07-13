namespace NBALigaSimulation.Client.Pages.Playoffs
{
	partial class Playoffs
	{
		private string message;
		private Dictionary<int, PlayoffsDto> series = new Dictionary<int, PlayoffsDto>();

		protected override async Task OnInitializedAsync()
		{
			message = "Carregando PLAYOFFS...";

			var result = await PlayoffsService.GetPlayoffs();
			if (!result.Success)
			{
				message = result.Message;
			}
			else
			{
				var playoffs = result.Data;

				if (playoffs != null)
				{
					foreach (var playoff in playoffs)
					{
						series[playoff.SeriesId] = playoff;
					}
				}
			}
		}

		private string CheckWinner(int wins)
		{
			return wins == 4 ? "bold-span" : "";
		}
	}

}
