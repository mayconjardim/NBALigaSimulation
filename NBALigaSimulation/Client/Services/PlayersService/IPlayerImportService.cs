using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.PlayersService;

public interface IPlayerImportService
{
    Task<ServiceResponse<PlayerCompleteDto>> CreatePlayer(CreatePlayerDto request);
    Task<ServiceResponse<bool>> CreatePlayers(List<CreatePlayerDto> playersDto);
    Task<ServiceResponse<bool>> ImportBBGMPlayers(BBGMImportDto bbgmData);
}
