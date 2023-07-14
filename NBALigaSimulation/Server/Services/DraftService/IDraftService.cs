namespace NBALigaSimulation.Server.Services.DraftService
{
	public interface IDraftService
	{
		Task<ServiceResponse<DraftLotteryDto>> GetLastLottery();
		Task<ServiceResponse<bool>> GenerateLottery();
	}
}
