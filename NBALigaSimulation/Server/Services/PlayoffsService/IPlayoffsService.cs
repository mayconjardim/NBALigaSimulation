using Microsoft.AspNetCore.Mvc;

namespace NBALigaSimulation.Server.Services.PlayoffsService
{
	public interface IPlayoffsService
	{

		Task<ServiceResponse<List<PlayoffsDto>>> GetPlayoffs();
		Task<ServiceResponse<bool>> GeneratePlayoffs();
		Task<ServiceResponse<bool>> Generate2Round();
		Task<ServiceResponse<bool>> Generate3Round();

	}
}
