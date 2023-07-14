namespace NBALigaSimulation.Client.Services.DraftService
{
	public interface IDraftService
	{

		Task<ServiceResponse<DraftLotteryDto>> GetLastLottery();
		Task<ServiceResponse<bool>> GenerateLottery();

	}
}
