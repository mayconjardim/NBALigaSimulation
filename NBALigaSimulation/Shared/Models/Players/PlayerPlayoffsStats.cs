using System.Globalization;
namespace NBALigaSimulation.Shared.Models.Players
{
	public class PlayerPlayoffsStats
	{

		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public int PlayerId { get; set; }
		public Player Player { get; set; }
		public int TeamId { get; set; }
		public string TeamAbrv { get; set; }
		public int Season { get; set; }
		public int Games { get; set; }
		public int Gs { get; set; }
		public double Min { get; set; }
		public int Fg { get; set; }
		public int Fga { get; set; }
		public int FgAtRim { get; set; }
		public int FgaAtRim { get; set; }
		public int FgLowPost { get; set; }
		public int FgaLowPost { get; set; }
		public int FgMidRange { get; set; }
		public int FgaMidRange { get; set; }
		public int Tp { get; set; }
		public int Tpa { get; set; }
		public int Ft { get; set; }
		public int Fta { get; set; }
		public int Orb { get; set; }
		public int Drb { get; set; }
		public int Ast { get; set; }
		public int Tov { get; set; }
		public int Stl { get; set; }
		public int Blk { get; set; }
		public int Pf { get; set; }
		public int Pts { get; set; }
		public int Trb { get; set; }

		public string PtsPG
		{
			get
			{
				double PtsPg = (double)Pts / Games;
				return PtsPg.ToString("0.0", CultureInfo.InvariantCulture);

			}
		}

		public string StlPG
		{
			get
			{
				double stlPg = (double)Stl / Games;
				return stlPg.ToString("0.0", CultureInfo.InvariantCulture);
			}
		}

		// Block
		public string BlkPG
		{
			get
			{
				double blkPg = (double)Blk / Games;
				return blkPg.ToString("0.0", CultureInfo.InvariantCulture);
			}
		}

		public string TRebPG
		{
			get
			{
				double TRebpg = (double)Trb / Games;
				return TRebpg.ToString("0.0", CultureInfo.InvariantCulture);
			}
		}

		// Ast
		public string AstPG
		{
			get
			{
				double astPg = (double)Ast / Games;
				return astPg.ToString("0.0", CultureInfo.InvariantCulture);
			}
		}
	}
}
