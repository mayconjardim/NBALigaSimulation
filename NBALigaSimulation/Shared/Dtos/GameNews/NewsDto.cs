﻿namespace NBALigaSimulation.Shared.Dtos.GameNews
{
    public class NewsDto
    {

        public int Id { get; set; }
        public int? GameId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Winner { get; set; } = string.Empty;

    }
}
