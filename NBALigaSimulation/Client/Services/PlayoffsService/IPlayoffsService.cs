namespace NBALigaSimulation.Client.Services.PlayoffsService
{
    public interface IPlayoffsService
    {

        Task<ServiceResponse<List<PlayoffsDto>>> GetPlayoffs();
        Task<ServiceResponse<PlayoffsDto>> GetPlayoffsById(int Id);
        Task<ServiceResponse<bool>> GeneratePlayoffs();

    }
}
