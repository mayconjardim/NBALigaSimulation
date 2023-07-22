using System.Threading.Tasks;

namespace NBALigaSimulation.Server.Services.SeasonService
{
    public interface ISeasonService
    {

        Task<ServiceResponse<CompleteSeasonDto>> GetLastSeason();
        Task<ServiceResponse<CompleteSeasonDto>> CreateSeason();
        Task<ServiceResponse<CompleteSeasonDto>> GenerateSchedule();
        Task<ServiceResponse<CompleteSeasonDto>> GenerateTrainingCamp();

    }
}
