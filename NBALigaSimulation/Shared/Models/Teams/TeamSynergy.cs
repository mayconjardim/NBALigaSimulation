﻿using System.ComponentModel.DataAnnotations.Schema;

namespace NBALigaSimulation.Shared.Models.Teams
{
    [NotMapped]
    public class TeamSynergy
    {

        public double Off { get; set; }
        public double Def { get; set; }
        public double Reb { get; set; }

    }
}
