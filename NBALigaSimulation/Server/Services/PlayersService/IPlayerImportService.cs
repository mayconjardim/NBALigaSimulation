using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.PlayersService
{
    public interface IPlayerImportService
    {
        Task<ServiceResponse<bool>> ImportBBGMPlayers(BBGMImportDto bbgmData);
        Task<ServiceResponse<PlayerCompleteDto>> CreatePlayer(CreatePlayerDto request);
        Task<ServiceResponse<bool>> CreatePlayers(List<CreatePlayerDto> playersDto);
    }
}
