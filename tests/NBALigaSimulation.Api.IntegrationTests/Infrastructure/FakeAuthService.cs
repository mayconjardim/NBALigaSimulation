using NBALigaSimulation.Server.Services.AuthService;
using NBALigaSimulation.Shared.Models.Users;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Api.IntegrationTests.Infrastructure;

public class FakeAuthService : IAuthService
{
    public int CurrentUserId { get; set; } = 100;
    public string CurrentUsername { get; set; } = "integration-user";

    public int GetUserId() => CurrentUserId;
    public string GetUsername() => CurrentUsername;

    public Task<ServiceResponse<SuccessfullyLogin>> Login(string email, string password)
        => throw new NotSupportedException();

    public Task<ServiceResponse<int>> Register(User user, string password)
        => throw new NotSupportedException();

    public Task<bool> UserExists(string email)
        => throw new NotSupportedException();

    public Task<ServiceResponse<bool>> ChangePassword(int userId, string newPassword)
        => throw new NotSupportedException();
}
