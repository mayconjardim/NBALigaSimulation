namespace NBALigaSimulation.Client.Services.SeasonService
{
    public class SeasonService : ISeasonService
    {

        private readonly HttpClient _http;

        public SeasonService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ServiceResponse<CompleteSeasonDto>> GetLastSeason()
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<CompleteSeasonDto>>($"api/seasons");
            return response;
        }

        public async Task<ServiceResponse<CompleteSeasonDto>> CreateSeason()
        {
            var response = new ServiceResponse<CompleteSeasonDto>();

            var payload = new CreateSeasonDto();

            var result = await _http.PostAsJsonAsync("api/seasons", payload);

            if (result.IsSuccessStatusCode)
            {
                var tradeResponse = await result.Content.ReadFromJsonAsync<ServiceResponse<CompleteSeasonDto>>();

                if (tradeResponse.Success)
                {
                    response.Success = true;
                    response.Data = tradeResponse.Data;
                    response.Message = "Temporada criada com sucesso.";
                }
                else
                {
                    response.Success = false;
                    response.Message = tradeResponse.Message;
                }
            }
            else
            {
                response.Success = false;
                response.Message = "Failed to create trade. HTTP status code: " + result.StatusCode;
            }

            return response;
        }



    }
}
