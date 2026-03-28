using System.Net.Http.Json;
using System.Text.Json;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Utilities;

public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

    /// <summary>
    /// Realiza requisição GET mapeando o retorno sempre para um ServiceResponse amigável.
    /// Evita o throw de HttpRequestException nativo do GetFromJsonAsync quando a API devolve falhas 4xx/5xx encapsuladas.
    /// </summary>
    public static async Task<ServiceResponse<T>> SafeGetAsync<T>(this HttpClient http, string url)
    {
        try
        {
            var response = await http.GetAsync(url);
            
            // Tentamos ler como ServiceResponse<T> independentemente do status code. 
            // O GlobalExceptionMiddleware injeta respostas no padrão ServiceResponse.
            try
            {
                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<T>>(Options);
                if (result != null) return result;
            }
            catch
            {
                // Se a leitura JSON falhar, trataremos na rotina de fallback abaixo
            }

            // Fallback para erros puros do servidor (IIS, Nginx) que não passaram pelo Middleware
            if (!response.IsSuccessStatusCode)
            {
                string rawError = await response.Content.ReadAsStringAsync();
                return new ServiceResponse<T> 
                { 
                    Success = false, 
                    Message = string.IsNullOrWhiteSpace(rawError) ? $"Erro HTTP {response.StatusCode}" : rawError 
                };
            }

            return new ServiceResponse<T> { Success = false, Message = "Resposta nula ou inválida do servidor." };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<T> { Success = false, Message = $"Erro de conexão: {ex.Message}" };
        }
    }

    /// <summary>
    /// Realiza POST serializado evitando throws prematuros.
    /// </summary>
    public static async Task<ServiceResponse<TResponse>> SafePostAsync<TRequest, TResponse>(this HttpClient http, string url, TRequest data)
    {
        try
        {
            var response = await http.PostAsJsonAsync(url, data);

            try
            {
                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<TResponse>>(Options);
                if (result != null) return result;
            }
            catch { }

            if (!response.IsSuccessStatusCode)
            {
                string rawError = await response.Content.ReadAsStringAsync();
                return new ServiceResponse<TResponse> { Success = false, Message = rawError };
            }

            return new ServiceResponse<TResponse> { Success = false, Message = "Resposta pós-envio nula ou inválida." };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<TResponse> { Success = false, Message = $"Erro de conexão: {ex.Message}" };
        }
    }

    /// <summary>
    /// Realiza PUT serializado
    /// </summary>
    public static async Task<ServiceResponse<TResponse>> SafePutAsync<TRequest, TResponse>(this HttpClient http, string url, TRequest data)
    {
        try
        {
            var response = await http.PutAsJsonAsync(url, data);

            try
            {
                 var result = await response.Content.ReadFromJsonAsync<ServiceResponse<TResponse>>(Options);
                 if (result != null) return result;
            }
            catch { }

            if (!response.IsSuccessStatusCode)
            {
                string rawError = await response.Content.ReadAsStringAsync();
                return new ServiceResponse<TResponse> { Success = false, Message = rawError };
            }

            return new ServiceResponse<TResponse> { Success = false, Message = "Resposta pós-atualização nula ou inválida." };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<TResponse> { Success = false, Message = $"Erro de conexão: {ex.Message}" };
        }
    }
}
