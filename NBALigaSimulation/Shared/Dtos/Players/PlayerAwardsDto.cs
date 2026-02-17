namespace NBALigaSimulation.Shared.Dtos.Players
{
	public class PlayerAwardsDto
	{

		public int Id { get; set; }
		public string Award { get; set; }
		public int Season { get; set; }
		public string Team { get; set; }
		public string PlayerName { get; set; } = string.Empty;
		public double Ppg { get; set; }
		public double Rpg { get; set; }
		public double Apg { get; set; }
		public double Spg { get; set; }
		public double Bpg { get; set; }

	}
}
