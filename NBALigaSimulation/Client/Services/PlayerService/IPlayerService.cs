
namespace NBALigaSimulation.Client.Services.PlayerService
{
    public interface IPlayerService
    {
        Task<ServiceResponse<PlayerCompleteDto>> GetProductById(int productId);

    }
}
