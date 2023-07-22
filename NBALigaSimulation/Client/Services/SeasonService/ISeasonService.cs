namespace NBALigaSimulation.Client.Services.SeasonService
{
    public interface ISeasonService
    {

        Task<ServiceResponse<CompleteSeasonDto>> GetLastSeason();
        Task<ServiceResponse<CompleteSeasonDto>> CreateSeason();
        Task<ServiceResponse<CompleteSeasonDto>> GenerateTrainingCamp();
        Task<ServiceResponse<CompleteSeasonDto>> GenerateSchedule();

    }
}
