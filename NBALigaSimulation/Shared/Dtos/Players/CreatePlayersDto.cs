using NBALigaSimulation.Shared.Dtos.Drafts;

namespace NBALigaSimulation.Shared.Dtos.Players;

public class CreatePlayersDto
{
    public int Pid { get; set; }
    public string Pos { get; set; }
    public string College { get; set; }
    public BornDto Born { get; set; }
    public int Weight { get; set; }
    public int Hgt { get; set; }
    public int TeamId { get; set; }
    public string ImgURL { get; set; }
    public bool Real { get; set; }
    public DraftDto Draft { get; set; }
    public List<PlayerRatingsDto>? Ratings { get; set; }
    public List<PlayerStatsDto>? Stats { get; set; }
    public InjuryDto? Injury { get; set; }
    public ContractDto? Contract { get; set; }
    public List<SalaryDto>? Salaries { get; set; }
    public List<AwardDto>? Awards { get; set; }
    public string JerseyNumber { get; set; }
    public string SrID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<string> Injuries { get; set; }
    public List<string> MoodTraits { get; set; }
    public List<string> Relatives { get; set; }
    public int ExportedSeason { get; set; }
}

public class BornDto
{
    public int Year { get; set; }
    public string Loc { get; set; }
}

public class PlayerRatingsDto
{
    public int Hgt { get; set; }
    public int Stre { get; set; }
    public int Spd { get; set; }
    public int Jmp { get; set; }
    public int Endu { get; set; }
    public int Ins { get; set; }
    public int Dnk { get; set; }
    public int Ft { get; set; }
    public int Fg { get; set; }
    public int Tp { get; set; }
    public int Diq { get; set; }
    public int Oiq { get; set; }
    public int Drb { get; set; }
    public int Pss { get; set; }
    public int Reb { get; set; }
    public int Season { get; set; }
    public string Pos { get; set; }
    public double Fuzz { get; set; }
    public List<string> Skills { get; set; }
    public int Ovr { get; set; }
    public int Pot { get; set; }
}

public class PlayerStatsDto
{
    public bool Playoffs { get; set; }
    public int Season { get; set; }
    public int Tid { get; set; }
    public int YearsWithTeam { get; set; }
    public double Per { get; set; }
    public double Ewa { get; set; }
    public double Astp { get; set; }
    public double Blkp { get; set; }
    public double Drbp { get; set; }
    public double Orbp { get; set; }
    public double Stlp { get; set; }
    public double Trbp { get; set; }
    public double Usgp { get; set; }
    public double Drtg { get; set; }
    public double Ortg { get; set; }
    public double Pm100 { get; set; }
    public double OnOff100 { get; set; }
    public double Dws { get; set; }
    public double Ows { get; set; }
    public double Obpm { get; set; }
    public double Dbpm { get; set; }
    public double Vorp { get; set; }
    public int Gp { get; set; }
    public int Gs { get; set; }
    public int Min { get; set; }
    public int MinAvailable { get; set; }
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
    public int Pm { get; set; }
    public int Orb { get; set; }
    public int Drb { get; set; }
    public int Ast { get; set; }
    public int Tov { get; set; }
    public int Stl { get; set; }
    public int Blk { get; set; }
    public int Ba { get; set; }
    public int Pf { get; set; }
    public int Pts { get; set; }
    public int Dd { get; set; }
    public int Td { get; set; }
    public int Qd { get; set; }
}

public class InjuryDto
{
    public string Type { get; set; }
    public int GamesRemaining { get; set; }
}

public class ContractDto
{
    public int Amount { get; set; }
    public int Exp { get; set; }
}

public class SalaryDto
{
    public int Amount { get; set; }
    public int Season { get; set; }
}

public class AwardDto
{
    public int Season { get; set; }
    public string Type { get; set; }
}

