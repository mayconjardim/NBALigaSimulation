using Newtonsoft.Json;

namespace NBALigaSimulation.Shared.Dtos.Players;

public class BBGMImportDto
{
    [JsonProperty("players")]
    public List<BBGMPlayerDto> Players { get; set; } = new();
}

public class BBGMPlayerDto
{
    [JsonProperty("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonProperty("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonProperty("pos")]
    public string Pos { get; set; } = string.Empty;

    [JsonProperty("college")]
    public string College { get; set; } = string.Empty;

    [JsonProperty("born")]
    public BBGMBornDto? Born { get; set; }

    [JsonProperty("hgt")]
    public int Hgt { get; set; }

    [JsonProperty("weight")]
    public int Weight { get; set; }

    [JsonProperty("imgURL")]
    public string ImgURL { get; set; } = string.Empty;

    [JsonProperty("tid")]
    public int Tid { get; set; }

    [JsonProperty("ratings")]
    public List<BBGMRatingDto>? Ratings { get; set; }
}

public class BBGMBornDto
{
    [JsonProperty("year")]
    public int Year { get; set; }

    [JsonProperty("loc")]
    public string Loc { get; set; } = string.Empty;
}

public class BBGMRatingDto
{
    [JsonProperty("hgt")]
    public int Hgt { get; set; }

    [JsonProperty("stre")]
    public int Stre { get; set; }

    [JsonProperty("spd")]
    public int Spd { get; set; }

    [JsonProperty("jmp")]
    public int Jmp { get; set; }

    [JsonProperty("endu")]
    public int Endu { get; set; }

    [JsonProperty("ins")]
    public int Ins { get; set; }

    [JsonProperty("dnk")]
    public int Dnk { get; set; }

    [JsonProperty("ft")]
    public int Ft { get; set; }

    [JsonProperty("fg")]
    public int Fg { get; set; }

    [JsonProperty("tp")]
    public int Tp { get; set; }

    [JsonProperty("diq")]
    public int Diq { get; set; }

    [JsonProperty("oiq")]
    public int Oiq { get; set; }

    [JsonProperty("drb")]
    public int Drb { get; set; }

    [JsonProperty("pss")]
    public int Pss { get; set; }

    [JsonProperty("reb")]
    public int Reb { get; set; }

    [JsonProperty("season")]
    public int Season { get; set; }

    [JsonProperty("pos")]
    public string Pos { get; set; } = string.Empty;

    [JsonProperty("skills")]
    public List<string>? Skills { get; set; }

    [JsonProperty("ovr")]
    public int Ovr { get; set; }

    [JsonProperty("pot")]
    public int Pot { get; set; }
}
