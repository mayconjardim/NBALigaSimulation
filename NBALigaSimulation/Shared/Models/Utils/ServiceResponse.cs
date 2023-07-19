
using NBALigaSimulation.Shared.Dtos;

namespace NBALigaSimulation.Shared.Models
{
    public class ServiceResponse<T>
    {

        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;

        public static implicit operator ServiceResponse<T>(ServiceResponse<List<PlayerSimpleDto>> v)
        {
            throw new NotImplementedException();
        }
    }
}
