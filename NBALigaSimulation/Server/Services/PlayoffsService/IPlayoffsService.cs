namespace NBALigaSimulation.Server.Services.PlayoffsService
{
    public interface IPlayoffsService
    {

        Task<ServiceResponse<List<PlayoffsDto>>> GetPlayoffs();
        Task<ServiceResponse<PlayoffsDto>> GetPlayoffsById(int Id);
        Task<ServiceResponse<bool>> GeneratePlayoffs();
        Task<ServiceResponse<bool>> Generate2Round();
        Task<ServiceResponse<bool>> Generate3Round();
        Task<ServiceResponse<bool>> Generate4Round();
        Task<ServiceResponse<bool>> EndPlayoffs();

    }
}
