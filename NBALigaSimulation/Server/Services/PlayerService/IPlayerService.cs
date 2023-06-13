namespace NBALigaSimulation.Server.Services.PlayerService
{
	public interface IPlayerService
	{

		Task<ServiceResponse<List<PlayerSimpleDto>>> GetAllPlayers();
		Task<ServiceResponse<PlayerCompleteDto>> GetPlayerById(int playerId);
		Task<ServiceResponse<PlayerCompleteDto>> CreatePlayer(CreatePlayerDto request);
		Task<ServiceResponse<List<PlayerCompleteDto>>> CreatePlayers(List<CreatePlayerDto> request);
		Task<bool> PlayerExists(int playerId);
		Task UpdatePlayerRosterOrder(int playerId, int newRosterOrder);

	}
}
