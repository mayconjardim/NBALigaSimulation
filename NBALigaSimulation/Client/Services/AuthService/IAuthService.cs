using NBALigaSimulation.Shared.Models.Users;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.AuthService;

public interface IAuthService
{
    
    Task<ServiceResponse<SuccessfullyLogin>> Login(UserLogin request);
    Task<ServiceResponse<bool>> ChangePassword(UserChangePassword request);
    
}