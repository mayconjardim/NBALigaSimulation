using NBALigaSimulation.Shared.Models.Games;

namespace NBALigaSimulation.Shared.Models.SeasonPlayoffs
{
	public class PlayoffsGame
	{

		public int PlayoffsId { get; set; }
		public Playoffs Playoffs { get; set; }
		public int GameId { get; set; }
		public Game Game { get; set; }

	}
}
