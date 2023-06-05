using NBALigaSimulation.Client.Pages.User;

namespace NBALigaSimulation.Client.Services.AuthService
{
    public class AuthService : IAuthService
    {

        private readonly HttpClient _http;

        public AuthService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ServiceResponse<int>> Register(UserRegister request)
        {
            var result = await _http.PostAsJsonAsync("api/auth/register", request);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<int>>();
        }

    }
}
