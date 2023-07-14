namespace NBALigaSimulation.Shared.Models
{
	public class PlayerAwards
	{

		public int Id { get; set; }
		public Player Player { get; set; }
		public int PlayerId { get; set; }
		public string Award { get; set; }
		public int Season { get; set; }
		public string Team { get; set; }
		public string Ppg { get; set; }
		public string Rpg { get; set; }
		public string Apg { get; set; }
		public string Spg { get; set; }
		public string Bpg { get; set; }

	}
}
