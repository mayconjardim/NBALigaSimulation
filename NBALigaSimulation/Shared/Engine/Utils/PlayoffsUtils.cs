using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Shared.Engine.Utils
{
	public static class PlayoffsUtils
	{


		public static List<Playoffs> Generate1stRound(List<Team> east, List<Team> west, int season)
		{

			List<Playoffs> playoffs = new List<Playoffs>();

			Playoffs Series1 = new Playoffs
			{
				Season = season,
				SeriesId = 1,
				Complete = false,
				TeamOneId = east.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 1))?.Id ?? 0,
				TeamTwoId = east.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 8))?.Id ?? 0,
				WinsTeamOne = 0,
				WinsTeamTwo = 0
			};

			playoffs.Add(Series1);

			Playoffs Series2 = new Playoffs
			{
				Season = season,
				SeriesId = 2,
				Complete = false,
				TeamOneId = east.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 4))?.Id ?? 0,
				TeamTwoId = east.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 5))?.Id ?? 0,
				WinsTeamOne = 0,
				WinsTeamTwo = 0
			};

			playoffs.Add(Series2);

			Playoffs Series3 = new Playoffs
			{
				Season = season,
				SeriesId = 3,
				Complete = false,
				TeamOneId = east.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 3))?.Id ?? 0,
				TeamTwoId = east.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 6))?.Id ?? 0,
				WinsTeamOne = 0,
				WinsTeamTwo = 0
			};

			playoffs.Add(Series3);

			Playoffs Series4 = new Playoffs
			{
				Season = season,
				SeriesId = 4,
				Complete = false,
				TeamOneId = east.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 2))?.Id ?? 0,
				TeamTwoId = east.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 7))?.Id ?? 0,
				WinsTeamOne = 0,
				WinsTeamTwo = 0
			};

			playoffs.Add(Series4);

			Playoffs Series5 = new Playoffs
			{
				Season = season,
				SeriesId = 5,
				Complete = false,
				TeamOneId = west.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 1))?.Id ?? 0,
				TeamTwoId = west.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 8))?.Id ?? 0,
				WinsTeamOne = 0,
				WinsTeamTwo = 0
			};

			playoffs.Add(Series5);

			Playoffs Series6 = new Playoffs
			{
				Season = season,
				SeriesId = 6,
				Complete = false,
				TeamOneId = west.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 4))?.Id ?? 0,
				TeamTwoId = west.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 5))?.Id ?? 0,
				WinsTeamOne = 0,
				WinsTeamTwo = 0
			};

			playoffs.Add(Series6);

			Playoffs Series7 = new Playoffs
			{
				Season = season,
				SeriesId = 7,
				Complete = false,
				TeamOneId = west.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 3))?.Id ?? 0,
				TeamTwoId = west.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 6))?.Id ?? 0,
				WinsTeamOne = 0,
				WinsTeamTwo = 0
			};

			playoffs.Add(Series7);


			Playoffs Series8 = new Playoffs
			{
				Season = season,
				SeriesId = 8,
				Complete = false,
				TeamOneId = west.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 2))?.Id ?? 0,
				TeamTwoId = west.FirstOrDefault(t => t.TeamRegularStats.Any(trs => trs.Season == season && trs.ConfRank == 7))?.Id ?? 0,
				WinsTeamOne = 0,
				WinsTeamTwo = 0
			};

			playoffs.Add(Series8);

			return playoffs;

		}

		public static List<Playoffs> Generate2ndRound(List<Playoffs> playoffsSeries, int season)
		{

			var winnerSerie1 = playoffsSeries
			   .Where(s => s.SeriesId == 1 && s.Season == season && (s.WinsTeamOne == 4 || s.WinsTeamTwo == 4))
			   .Select(s => s.WinsTeamOne == 4 ? s.TeamOne : s.TeamTwo)
			   .FirstOrDefault();

			var winnerSerie2 = playoffsSeries
			   .Where(s => s.SeriesId == 2 && s.Season == season && (s.WinsTeamOne == 4 || s.WinsTeamTwo == 4))
			   .Select(s => s.WinsTeamOne == 4 ? s.TeamOne : s.TeamTwo)
			   .FirstOrDefault();

			var winnerSerie3 = playoffsSeries
			   .Where(s => s.SeriesId == 3 && s.Season == season && (s.WinsTeamOne == 4 || s.WinsTeamTwo == 4))
			   .Select(s => s.WinsTeamOne == 4 ? s.TeamOne : s.TeamTwo)
			   .FirstOrDefault();

			var winnerSerie4 = playoffsSeries
			   .Where(s => s.SeriesId == 4 && s.Season == season && (s.WinsTeamOne == 4 || s.WinsTeamTwo == 4))
			   .Select(s => s.WinsTeamOne == 4 ? s.TeamOne : s.TeamTwo)
			   .FirstOrDefault();

			var winnerSerie5 = playoffsSeries
			   .Where(s => s.SeriesId == 5 && s.Season == season && (s.WinsTeamOne == 4 || s.WinsTeamTwo == 4))
			   .Select(s => s.WinsTeamOne == 4 ? s.TeamOne : s.TeamTwo)
			   .FirstOrDefault();

			var winnerSerie6 = playoffsSeries
			   .Where(s => s.SeriesId == 6 && s.Season == season && (s.WinsTeamOne == 4 || s.WinsTeamTwo == 4))
			   .Select(s => s.WinsTeamOne == 4 ? s.TeamOne : s.TeamTwo)
			   .FirstOrDefault();

			var winnerSerie7 = playoffsSeries
			   .Where(s => s.SeriesId == 7 && s.Season == season && (s.WinsTeamOne == 4 || s.WinsTeamTwo == 4))
			   .Select(s => s.WinsTeamOne == 4 ? s.TeamOne : s.TeamTwo)
			   .FirstOrDefault();

			var winnerSerie8 = playoffsSeries
			   .Where(s => s.SeriesId == 8 && s.Season == season && (s.WinsTeamOne == 4 || s.WinsTeamTwo == 4))
			   .Select(s => s.WinsTeamOne == 4 ? s.TeamOne : s.TeamTwo)
			   .FirstOrDefault();

			List<Playoffs> playoffs = new List<Playoffs>();

			int teamOneId;
			int teamTwoId;

			if (winnerSerie1.TeamRegularStats.Where(t => t.Season == season).LastOrDefault().ConfRank > winnerSerie2.TeamRegularStats.Where(t => t.Season == season).LastOrDefault().ConfRank)
			{
				teamOneId = winnerSerie1.Id;
				teamTwoId = winnerSerie2.Id;
			}
			else
			{
				teamOneId = winnerSerie2.Id;
				teamTwoId = winnerSerie1.Id;
			}

			var Series9 = new Playoffs
			{
				Season = season,
				SeriesId = 9,
				Complete = false,
				TeamOneId = teamOneId,
				TeamTwoId = teamTwoId,
				WinsTeamOne = 0,
				WinsTeamTwo = 0
			};

			playoffs.Add(Series9);


			if (winnerSerie3.TeamRegularStats.Where(t => t.Season == season).LastOrDefault().ConfRank > winnerSerie4.TeamRegularStats.Where(t => t.Season == season).LastOrDefault().ConfRank)
			{
				teamOneId = winnerSerie3.Id;
				teamTwoId = winnerSerie4.Id;
			}
			else
			{
				teamOneId = winnerSerie4.Id;
				teamTwoId = winnerSerie3.Id;
			}

			Playoffs Series10 = new Playoffs
			{
				Season = season,
				SeriesId = 10,
				Complete = false,
				TeamOneId = teamOneId,
				TeamTwoId = teamTwoId,
				WinsTeamOne = 0,
				WinsTeamTwo = 0
			};

			playoffs.Add(Series10);


			if (winnerSerie5.TeamRegularStats.Where(t => t.Season == season).LastOrDefault().ConfRank > winnerSerie6.TeamRegularStats.Where(t => t.Season == season).LastOrDefault().ConfRank)
			{
				teamOneId = winnerSerie5.Id;
				teamTwoId = winnerSerie6.Id;
			}
			else
			{
				teamOneId = winnerSerie6.Id;
				teamTwoId = winnerSerie5.Id;
			}

			Playoffs Series11 = new Playoffs
			{
				Season = season,
				SeriesId = 11,
				Complete = false,
				TeamOneId = teamOneId,
				TeamTwoId = teamTwoId,
				WinsTeamOne = 0,
				WinsTeamTwo = 0
			};

			playoffs.Add(Series11);


			if (winnerSerie7.TeamRegularStats.Where(t => t.Season == season).LastOrDefault().ConfRank > winnerSerie8.TeamRegularStats.Where(t => t.Season == season).LastOrDefault().ConfRank)
			{
				teamOneId = winnerSerie7.Id;
				teamTwoId = winnerSerie8.Id;
			}
			else
			{
				teamOneId = winnerSerie8.Id;
				teamTwoId = winnerSerie7.Id;
			}

			Playoffs Series12 = new Playoffs
			{
				Season = season,
				SeriesId = 12,
				Complete = false,
				TeamOneId = teamOneId,
				TeamTwoId = teamTwoId,
				WinsTeamOne = 0,
				WinsTeamTwo = 0
			};

			playoffs.Add(Series12);

			return playoffs;
		}


		public static List<Playoffs> Generate3ndRound(List<Playoffs> playoffsSeries, int season)
		{

			var winnerSerie9 = playoffsSeries
			   .Where(s => s.SeriesId == 9 && s.Season == season && (s.WinsTeamOne == 4 || s.WinsTeamTwo == 4))
			   .Select(s => s.WinsTeamOne == 4 ? s.TeamOne : s.TeamTwo)
			   .FirstOrDefault();

			var winnerSerie10 = playoffsSeries
			   .Where(s => s.SeriesId == 10 && s.Season == season && (s.WinsTeamOne == 4 || s.WinsTeamTwo == 4))
			   .Select(s => s.WinsTeamOne == 4 ? s.TeamOne : s.TeamTwo)
			   .FirstOrDefault();

			var winnerSerie11 = playoffsSeries
			   .Where(s => s.SeriesId == 11 && s.Season == season && (s.WinsTeamOne == 4 || s.WinsTeamTwo == 4))
			   .Select(s => s.WinsTeamOne == 4 ? s.TeamOne : s.TeamTwo)
			   .FirstOrDefault();

			var winnerSerie12 = playoffsSeries
			   .Where(s => s.SeriesId == 12 && s.Season == season && (s.WinsTeamOne == 4 || s.WinsTeamTwo == 4))
			   .Select(s => s.WinsTeamOne == 4 ? s.TeamOne : s.TeamTwo)
			   .FirstOrDefault();

			List<Playoffs> playoffs = new List<Playoffs>();

			int teamOneId;
			int teamTwoId;

			if (winnerSerie9.TeamRegularStats.Where(t => t.Season == season).LastOrDefault().ConfRank > winnerSerie10.TeamRegularStats.Where(t => t.Season == season).LastOrDefault().ConfRank)
			{
				teamOneId = winnerSerie9.Id;
				teamTwoId = winnerSerie10.Id;
			}
			else
			{
				teamOneId = winnerSerie10.Id;
				teamTwoId = winnerSerie9.Id;
			}

			var Series13 = new Playoffs
			{
				Season = season,
				SeriesId = 13,
				Complete = false,
				TeamOneId = teamOneId,
				TeamTwoId = teamTwoId,
				WinsTeamOne = 0,
				WinsTeamTwo = 0
			};

			playoffs.Add(Series13);


			if (winnerSerie11.TeamRegularStats.Where(t => t.Season == season).LastOrDefault().ConfRank > winnerSerie12.TeamRegularStats.Where(t => t.Season == season).LastOrDefault().ConfRank)
			{
				teamOneId = winnerSerie11.Id;
				teamTwoId = winnerSerie12.Id;
			}
			else
			{
				teamOneId = winnerSerie12.Id;
				teamTwoId = winnerSerie11.Id;
			}

			Playoffs Series14 = new Playoffs
			{
				Season = season,
				SeriesId = 14,
				Complete = false,
				TeamOneId = teamOneId,
				TeamTwoId = teamTwoId,
				WinsTeamOne = 0,
				WinsTeamTwo = 0
			};

			playoffs.Add(Series14);

			return playoffs;
		}

		public static List<Playoffs> Generate4ndRound(List<Playoffs> playoffsSeries, int season)
		{

			var winnerSerie13 = playoffsSeries
			   .Where(s => s.SeriesId == 13 && s.Season == season && (s.WinsTeamOne == 4 || s.WinsTeamTwo == 4))
			   .Select(s => s.WinsTeamOne == 4 ? s.TeamOne : s.TeamTwo)
			   .FirstOrDefault();

			var winnerSerie14 = playoffsSeries
			   .Where(s => s.SeriesId == 14 && s.Season == season && (s.WinsTeamOne == 4 || s.WinsTeamTwo == 4))
			   .Select(s => s.WinsTeamOne == 4 ? s.TeamOne : s.TeamTwo)
			   .FirstOrDefault();


			List<Playoffs> playoffs = new List<Playoffs>();

			int teamOneId;
			int teamTwoId;

			if (winnerSerie13.TeamRegularStats.Where(t => t.Season == season).LastOrDefault().ConfRank > winnerSerie14.TeamRegularStats.Where(t => t.Season == season).LastOrDefault().ConfRank)
			{
				teamOneId = winnerSerie13.Id;
				teamTwoId = winnerSerie14.Id;
			}
			else
			{
				teamOneId = winnerSerie14.Id;
				teamTwoId = winnerSerie13.Id;
			}

			var Series15 = new Playoffs
			{
				Season = season,
				SeriesId = 15,
				Complete = false,
				TeamOneId = teamOneId,
				TeamTwoId = teamTwoId,
				WinsTeamOne = 0,
				WinsTeamTwo = 0
			};

			playoffs.Add(Series15);
			return playoffs;
		}

		public static List<PlayoffsGame> GenerateRoundGames(List<Playoffs> playoffs, Season season)
		{
			List<PlayoffsGame> playoffGames = new List<PlayoffsGame>();

			DateTime dataInicial = DateTime.UtcNow.AddDays(1);
			TimeZoneInfo tzBrasilia = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
			dataInicial = new DateTime(dataInicial.Year, dataInicial.Month, dataInicial.Day, 23, 0, 0);
			dataInicial = TimeZoneInfo.ConvertTimeFromUtc(dataInicial, tzBrasilia);

			foreach (var serie in playoffs)
			{
				var playoffGame1 = new PlayoffsGame
				{
					Playoffs = serie,
					Game = new Game
					{
						Type = 1,
						HomeTeamId = serie.TeamOneId,
						AwayTeamId = serie.TeamTwoId,
						GameDate = dataInicial,
						Season = season
					}
				};

				playoffGames.Add(playoffGame1);

				var playoffGame2 = new PlayoffsGame
				{
					Playoffs = serie,
					Game = new Game
					{
						Type = 1,
						HomeTeamId = serie.TeamOneId,
						AwayTeamId = serie.TeamTwoId,
						GameDate = dataInicial,
						Season = season
					}
				};

				playoffGames.Add(playoffGame2);

				var playoffGame3 = new PlayoffsGame
				{
					Playoffs = serie,
					Game = new Game
					{
						Type = 1,
						HomeTeamId = serie.TeamTwoId,
						AwayTeamId = serie.TeamOneId,
						GameDate = dataInicial,
						Season = season
					}
				};

				playoffGames.Add(playoffGame3);

				var playoffGame4 = new PlayoffsGame
				{
					Playoffs = serie,
					Game = new Game
					{
						Type = 1,
						HomeTeamId = serie.TeamTwoId,
						AwayTeamId = serie.TeamOneId,
						GameDate = dataInicial,
						Season = season
					}
				};

				playoffGames.Add(playoffGame4);

				var playoffGame5 = new PlayoffsGame
				{
					Playoffs = serie,
					Game = new Game
					{
						Type = 1,
						HomeTeamId = serie.TeamOneId,
						AwayTeamId = serie.TeamTwoId,
						GameDate = dataInicial,
						Season = season
					}
				};

				playoffGames.Add(playoffGame5);

				var playoffGame6 = new PlayoffsGame
				{
					Playoffs = serie,
					Game = new Game
					{
						Type = 1,
						HomeTeamId = serie.TeamTwoId,
						AwayTeamId = serie.TeamOneId,
						GameDate = dataInicial,
						Season = season
					}
				};

				playoffGames.Add(playoffGame6);

				var playoffGame7 = new PlayoffsGame
				{
					Playoffs = serie,
					Game = new Game
					{
						Type = 1,
						HomeTeamId = serie.TeamOneId,
						AwayTeamId = serie.TeamTwoId,
						GameDate = dataInicial,
						Season = season
					}
				};

				playoffGames.Add(playoffGame7);
			}

			return playoffGames;
		}

	}
}
