using NBALigaSimulation.Shared.Models.Utils;
using User = NBALigaSimulation.Shared.Models.Users.User;

namespace NBALigaSimulation.Server.Services.AuthService
{
    public interface IAuthService
    {
        
        Task<ServiceResponse<SuccessfullyLogin>> Login(string email, string password);
        Task<ServiceResponse<int>> Register(User user, string password);
        Task<bool> UserExists(string email);
        Task<ServiceResponse<bool>> ChangePassword(int userId, string newPassword);
        int GetUserId();
        string GetUsername();
        

    }
}
