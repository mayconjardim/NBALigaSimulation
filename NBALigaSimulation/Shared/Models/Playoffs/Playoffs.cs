namespace NBALigaSimulation.Shared.Models
{
	public class Playoffs
	{

		public int Id { get; set; }
		public int Season { get; set; }
		public int SeriesId { get; set; }
		public bool? Complete { get; set; }
		public int TeamOneId { get; set; }
		public Team TeamOne { get; set; }

		public int TeamTwoId { get; set; }
		public Team TeamTwo { get; set; }

		public int WinsTeamOne { get; set; }
		public int WinsTeamTwo { get; set; }

		public List<PlayoffsGame> PlayoffGames { get; set; } = new List<PlayoffsGame>();

	}
}
