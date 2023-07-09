namespace NBALigaSimulation.Server.Services.PlayoffsService
{
    public interface IPlayoffsService
    {

        Task<ServiceResponse<PlayoffsDto>> GetPlayoffs();

    }
}
