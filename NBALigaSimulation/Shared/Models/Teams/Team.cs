using System.ComponentModel.DataAnnotations.Schema;
using NBALigaSimulation.Shared.Models.Players;

namespace NBALigaSimulation.Shared.Models.Teams
{
    public class Team
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Abrv { get; set; } = string.Empty;
        public string Conference { get; set; } = string.Empty;
        public bool IsHuman { get; set; } = true;
        public int Championships { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
        public List<TeamGameStats> Stats { get; set; } = new List<TeamGameStats>();
        public List<TeamDraftPicks> DraftPicks { get; set; } = new List<TeamDraftPicks>();
        public TeamGameplan Gameplan { get; set; }
        public List<TeamRegularStats> TeamRegularStats { get; set; } = new List<TeamRegularStats>();
        public List<TeamPlayoffsStats> TeamPlayoffsStats { get; set; } = new List<TeamPlayoffsStats>();

        [NotMapped]
        public TeamCompositeRating CompositeRating { get; set; }

        [NotMapped]
        public TeamSynergy Synergy { get; set; }

    }

}
