namespace NBALigaSimulation.Shared.Dtos.FA;

/// <summary>Resultado da simulação de uma rodada da free agency.</summary>
public class FASimulateRoundResultDto
{
    /// <summary>Quantidade de jogadores que assinaram.</summary>
    public int SigningsCount { get; set; }

    /// <summary>Lista de assinaturas: jogador, time, valor/anos.</summary>
    public List<FASigningDto> Signings { get; set; } = new List<FASigningDto>();

    public string Message { get; set; } = string.Empty;
}

public class FASigningDto
{
    public string PlayerName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public int Amount { get; set; }
    public int Years { get; set; }
}
