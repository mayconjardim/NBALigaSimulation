namespace NBALigaSimulation.Server.Services.DraftService
{
	public interface IDraftService
	{

		Task<ServiceResponse<bool>> GenerateLottery();

	}
}
