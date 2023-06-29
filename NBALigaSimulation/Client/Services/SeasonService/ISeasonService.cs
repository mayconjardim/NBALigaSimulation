namespace NBALigaSimulation.Client.Services.SeasonService
{
    public interface ISeasonService
    {
        Task<ServiceResponse<CompleteSeasonDto>> GetLastSeason();

    }
}
