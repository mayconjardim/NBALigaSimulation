using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NBALigaSimulation.Server.Services.GmService;
using NBALigaSimulation.Shared.Dtos.Gm;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Controllers;

[ApiController]
[Route("api/gm")]
public class GmController : ControllerBase
{
    private readonly IGmService _gmService;

    public GmController(IGmService gmService)
    {
        _gmService = gmService;
    }

    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<GmProfileDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ServiceResponse<GmProfileDto>>> GetMyProfile()
    {
        var result = await _gmService.GetMyProfile();
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("team/{teamId:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ServiceResponse<GmProfileDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ServiceResponse<GmProfileDto>>> GetProfileByTeamId(int teamId)
    {
        var result = await _gmService.GetProfileByTeamId(teamId);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }
}
