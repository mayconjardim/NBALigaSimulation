namespace NBALigaSimulation.Shared.Engine.Scouting;

public static class ScoutingProfiles
{
    public static Dictionary<string, PositionProfile> Profiles { get; } = new()
    {
        ["PG"] = new PositionProfile
        {
            CoreAttributes = { "Pss", "Drb", "Oiq", "Spd" },
            BonusAttributes = { "Tp", "Diq", "Reb" }
        },
        ["SG"] = new PositionProfile
        {
            CoreAttributes = { "Tp", "Mid", "Spd" },
            BonusAttributes = { "Pss", "Diq" }
        },
        ["SF"] = new PositionProfile
        {
            CoreAttributes = { "Ins", "Tp", "Diq" },
            BonusAttributes = { "Reb", "Pss", "Drb" }
        },
        ["PF"] = new PositionProfile
        {
            CoreAttributes = { "Ins", "Reb", "Stre" },
            BonusAttributes = { "Mid", "Tp" }
        },
        ["C"] = new PositionProfile
        {
            CoreAttributes = { "Ins", "Reb", "Stre", "Hgt" },
            BonusAttributes = { "Pss", "Mid", "Tp" }
        }
    };
}
