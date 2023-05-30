namespace NBALigaSimulation.Server.Services.SeasonService
{
    public interface ISeasonService
    {
        Task<ServiceResponse<CompleteSeasonDto>> CreateSeason(CreateSeasonDto request);
    }
}
