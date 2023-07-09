namespace NBALigaSimulation.Server.Services.PlayoffsService
{
    public interface IPlayoffsService
    {

        Task<ServiceResponse<List<PlayoffsDto>>> GetPlayoffs();
        Task<ServiceResponse<bool>> GeneratePlayoffs();

    }
}
