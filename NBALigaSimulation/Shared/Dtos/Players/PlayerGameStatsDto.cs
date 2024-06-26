﻿using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Shared.Dtos.Players
{
    public class PlayerGameStatsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Pos { get; set; } = string.Empty;
        public int PlayerId { get; set; }
        public int TeamId { get; set; }
        public int GameId { get; set; }
        public DateTime GameDate { get; set; }
        public string OppAbrev { get; set; } = string.Empty;
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
        public double CourtTime { get; set; }
        public double BenchTime { get; set; }
        public int Energy { get; set; }
    }
}
