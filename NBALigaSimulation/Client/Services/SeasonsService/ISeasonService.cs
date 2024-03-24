using NBALigaSimulation.Shared.Dtos.Seasons;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.SeasonsService;
 
    public interface ISeasonService
    {

        Task<ServiceResponse<CompleteSeasonDto>> GetLastSeason();
        Task<ServiceResponse<List<CompleteSeasonDto>>> GetALlSeason();

    }
    
