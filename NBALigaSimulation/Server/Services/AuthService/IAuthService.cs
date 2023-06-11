namespace NBALigaSimulation.Server.Services.AuthService
{
    public interface IAuthService
    {

        Task<ServiceResponse<int>> Register(User user, string password);
        Task<ServiceResponse<string>> Login(string email, string password);
        Task<bool> UserExists(string email);
        Task<ServiceResponse<bool>> ChangePassword(int userId, string newPassword);
        int GetUserId();
        string GetUsername();

    }
}
