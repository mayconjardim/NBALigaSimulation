using NBALigaSimulation.Shared.Models;
using System;

namespace NBALigaSimulation.Shared.Engine.Utils
{
	public static class DraftUtils
	{

		public static List<TeamRegularStats> RunLottery(List<TeamRegularStats> teams)
		{

			Dictionary<int, int> LOTTO_BALLS = new Dictionary<int, int>
			{

			{0, 250},
			{1, 199},
			{2, 156},
			{3, 119},
			{4, 88},
			{5, 63},

			};

			List<TeamRegularStats> order = new List<TeamRegularStats>();

			Random random = new Random();
			while (LOTTO_BALLS.Count > 0)
			{
				double N = LOTTO_BALLS.Values.Sum();
				int draw = weightedRandomChoice(LOTTO_BALLS.Keys.ToList(), LOTTO_BALLS.Values.ToList(), N);
				order.Add(teams[draw]);
				LOTTO_BALLS.Remove(draw);
			}

			return order;
		}

		static int weightedRandomChoice(List<int> choices, List<int> weights, double totalWeight)
		{
			Random random = new Random();
			double randomNumber = random.NextDouble() * totalWeight;
			double weightSum = 0;
			for (int i = 0; i < choices.Count; i++)
			{
				weightSum += weights[i];
				if (randomNumber < weightSum)
				{
					return choices[i];
				}
			}
			return choices.Last();
		}

	}
}
