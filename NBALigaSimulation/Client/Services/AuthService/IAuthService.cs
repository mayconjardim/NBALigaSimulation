using NBALigaSimulation.Client.Pages.User;

namespace NBALigaSimulation.Client.Services.AuthService
{
    public interface IAuthService
    {

        Task<ServiceResponse<int>> Register(UserRegister request);

    }
}
