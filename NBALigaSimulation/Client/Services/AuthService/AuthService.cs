﻿namespace NBALigaSimulation.Client.Services.AuthService
{
    public class AuthService : IAuthService
    {

        private readonly HttpClient _http;

        public AuthService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ServiceResponse<int>> Register(UserRegistration request)
        {
            var result = await _http.PostAsJsonAsync("api/auth/register", request);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<int>>();
        }

        public async Task<ServiceResponse<string>> Login(UserLogin request)
        {
            var result = await _http.PostAsJsonAsync("api/auth/login", request);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        }

        public async Task<ServiceResponse<bool>> ChangePassword(UserChangePassword request)
        {
            var result = await _http.PostAsJsonAsync("api/auth/change-password", request.Password);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }


    }
}
