using NBALigaSimulation.Shared.Dtos.Seasons;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.SeasonsService;
 
    public interface ISeasonService
    {

        Task<ServiceResponse<CompleteSeasonDto>> GetLastSeason();
        Task<ServiceResponse<List<CompleteSeasonDto>>> GetALlSeason();
        Task<ServiceResponse<CompleteSeasonDto>> GenerateSchedule();
        Task<ServiceResponse<CompleteSeasonDto>> CreateSeason();
        Task<ServiceResponse<CompleteSeasonDto>> CleanSchedule();
        Task<ServiceResponse<CompleteSeasonDto>> GenerateTrainingCamp();

    }
    
