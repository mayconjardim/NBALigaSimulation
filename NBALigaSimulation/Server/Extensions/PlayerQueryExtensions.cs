using System.Collections.Generic;
using System.Linq;
using NBALigaSimulation.Shared.Models.Players;

namespace NBALigaSimulation.Server.Extensions
{
    public static class PlayerQueryExtensions
    {
        public static IEnumerable<Player> ApplySorting(this IEnumerable<Player> players, string sortedColumn, bool isAscending)
        {
            var orderByExpression = players.OrderBy(p => p.Ratings.LastOrDefault()?.CalculateOvr);

            if (string.IsNullOrEmpty(sortedColumn)) return orderByExpression;

            return sortedColumn switch
            {
                "OVR" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.CalculateOvr) : players.OrderBy(p => p.Ratings.LastOrDefault()?.CalculateOvr),
                "POT" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Pot) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Pot),
                "HGT" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Hgt) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Hgt),
                "STR" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Stre) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Stre),
                "SPD" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Spd) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Spd),
                "JMP" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Jmp): players.OrderBy(p => p.Ratings.LastOrDefault()?.Jmp),
                "END" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Endu) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Endu),
                "INS" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Ins) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Ins),
                "DNK" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Dnk) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Dnk),
                "FT" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Ft) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Ft),
                "2PT" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Fg) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Fg),
                "3PT" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Tp) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Tp),
                "OIQ" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Oiq) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Oiq),
                "DIQ" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Diq) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Diq),
                "DRB" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Drb) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Drb),
                "PSS" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Pss) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Pss),
                "REB" => isAscending ? players.OrderByDescending(p => p.Ratings.LastOrDefault()?.Reb) : players.OrderBy(p => p.Ratings.LastOrDefault()?.Reb),
                _ => orderByExpression
            };
        }
    }
}
