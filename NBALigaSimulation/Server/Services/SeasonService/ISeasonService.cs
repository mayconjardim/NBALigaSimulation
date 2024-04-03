using System.Threading.Tasks;
using NBALigaSimulation.Shared.Dtos.Seasons;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.SeasonService
{
    public interface ISeasonService
    {

        Task<ServiceResponse<CompleteSeasonDto>> GetLastSeason();
        Task<ServiceResponse<List<CompleteSeasonDto>>> GetALlSeason();
        Task<ServiceResponse<CompleteSeasonDto>> CreateSeason();
        Task<ServiceResponse<CompleteSeasonDto>> GenerateSchedule();
        Task<ServiceResponse<CompleteSeasonDto>> CleanSchedule();
        Task<ServiceResponse<CompleteSeasonDto>> GenerateTrainingCamp();

    }
}
