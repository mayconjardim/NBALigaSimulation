﻿using Microsoft.AspNetCore.Components;

namespace NBALigaSimulation.Client.Pages.Team
{
	partial class TeamSchedule
	{

		[Parameter]
		public TeamCompleteDto? team { get; set; }
		List<GameCompleteDto> games { get; set; }
		private string message = string.Empty;


		private int gamehome;
		private int gamesaway;
		private int totalGames;

		string[] headings = { "HOME", "AWAY", "DATE", "RESULT" };

		protected override async Task OnParametersSetAsync()
		{
			message = "Carregando Jogos por Time...";

			var result = await GameService.GetGamesByTeamId(team.Id);
			if (!result.Success)
			{
				message = result.Message;
			}
			else
			{
				games = result.Data.OrderBy(t => t.GameDate).Where(t => t.Type == 1).ToList();


			}
		}

		protected string GetScoreClass(int homeScore, int awayScore)
		{
			if (homeScore > awayScore)
			{
				return "text-green";
			}
			else if (awayScore > homeScore)
			{
				return "text-red";
			}
			else
			{
				return "";
			}
		}


	}
}
