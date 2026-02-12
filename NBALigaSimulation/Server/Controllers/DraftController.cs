using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NBALigaSimulation.Shared.Dtos.Drafts;
using NBALigaSimulation.Shared.Models.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace NBALigaSimulation.Server.Controllers
{
    [Route("api/draft")]
    [ApiController]
    public class DraftController : ControllerBase
    {
        private readonly IDraftService _draftService;
        private readonly ITeamService _teamService;

        public DraftController(IDraftService draftService, ITeamService teamService)
        {
            _draftService = draftService;
            _teamService = teamService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "GetLastDraft", Description = "Retorna a ultimo draft realizado.")]
        public async Task<ActionResult<ServiceResponse<List<DraftDto>>>> GetLastDraft()
        {

            var result = await _draftService.GetLastDraft();

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

        [HttpGet("lotto")]
        [SwaggerOperation(Summary = "GetLastLottery", Description = "Retorna a ultima loteria realizada.")]
        public async Task<ActionResult<ServiceResponse<DraftLotteryDto>>> GetLastLottery()
        {

            var result = await _draftService.GetLastLottery();

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

        [HttpPost("generate/lottery")]
        public async Task<ActionResult<ServiceResponse<bool>>> GenerateLottery()
        {
            var result = await _draftService.GenerateLottery();
            return Ok(result);
        }

        [HttpPost("generate/draft")]
        public async Task<ActionResult<ServiceResponse<bool>>> GenerateDraft()
        {
            var result = await _draftService.GenerateDraft();
            return Ok(result);
        }

        [HttpPost("finalize")]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> FinalizeDraft()
        {
            var isAdminClaim = User.FindFirst("IsAdmin")?.Value;
            var isAdmin = string.Equals(isAdminClaim, "True", StringComparison.OrdinalIgnoreCase);
            if (!isAdmin)
                return StatusCode(403, new ServiceResponse<bool> { Success = false, Message = "Apenas o admin pode finalizar o draft." });

            var result = await _draftService.FinalizeDraft();
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("select")]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> SelectDraftedPlayer(DraftPlayerDto request)
        {
            var isAdminClaim = User.FindFirst("IsAdmin")?.Value;
            var isAdmin = string.Equals(isAdminClaim, "True", StringComparison.OrdinalIgnoreCase);
            if (!isAdmin)
            {
                var teamResult = await _teamService.GetTeamByLoggedUser();
                if (!teamResult.Success || teamResult.Data == null || teamResult.Data.Id != request.TeamId)
                    return StatusCode(403, new ServiceResponse<bool> { Success = false, Message = "Apenas o admin pode escolher para outros times, ou o dono do time na sua vez." });
            }

            var response = await _draftService.SelectDraftedPlayer(request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

    }
}
