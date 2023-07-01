using System.Threading.Tasks;

namespace NBALigaSimulation.Server.Services.SeasonService
{
    public interface ISeasonService
    {

        Task<ServiceResponse<CompleteSeasonDto>> GetLastSeason();
        Task<ServiceResponse<CompleteSeasonDto>> CreateSeason(CreateSeasonDto request);
        Task<ServiceResponse<CompleteSeasonDto>> GenerateSchedule(int seasonId);
        Task<ServiceResponse<CompleteSeasonDto>> GenerateTrainingCamp();

    }
}
