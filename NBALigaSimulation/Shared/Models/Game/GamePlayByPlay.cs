﻿namespace NBALigaSimulation.Shared.Models
{
    public class GamePlayByPlay
    {
        public int Id { get; set; }
        public int GameSimId { get; set; }
        public Game Game { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public double T { get; set; }
        public string Time { get; set; }

    }
}
