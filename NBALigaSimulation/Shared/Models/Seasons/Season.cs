using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Models.Seasons
{
    public class Season
    {

        public int Id { get; set; }
        public int Year { get; set; }
    
        public bool IsCompleted { get; set; } = false;
        public bool LotteryCompleted { get; set; } = false;
        public bool DraftCompleted { get; set; } = false;
        public bool TcCompleted { get; set; } = false;
        public bool DeadlineCompleted { get; set; } = false;
        public bool RegularCompleted { get; set; } = false;
        
        //Playoffs
        public bool FirstRoundCompleted { get; set; } = false;
        public bool SecondRoundCompleted { get; set; } = false;
        public bool ThirdRoundCompleted { get; set; } = false;
        public bool FourthRoundCompleted { get; set; } = false;
        
        public List<Game> Games { get; set; }

    }
}
