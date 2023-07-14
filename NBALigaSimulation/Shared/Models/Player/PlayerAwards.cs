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
		public double Ppg { get; set; }
		public double Rpg { get; set; }
		public double Apg { get; set; }
		public double Spg { get; set; }
		public double Bpg { get; set; }

	}
}
