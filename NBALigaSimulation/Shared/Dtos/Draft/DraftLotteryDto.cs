namespace NBALigaSimulation.Shared.Dtos
{
	public class DraftLotteryDto
	{

		public int Id { get; set; }
		public int Season { get; set; }
		public string FirstTeam { get; set; }
		public int FirstTeamId { get; set; }

		public string SecondTeam { get; set; }
		public int SecondTeamId { get; set; }

		public string ThirdTeam { get; set; }
		public int ThirdTeamId { get; set; }

		public string FourthTeam { get; set; }
		public int FourthTeamId { get; set; }

		public string FifthTeam { get; set; }
		public int FifthTeamId { get; set; }

		public string SixthTeam { get; set; }
		public int SixthTeamId { get; set; }

	}
}
