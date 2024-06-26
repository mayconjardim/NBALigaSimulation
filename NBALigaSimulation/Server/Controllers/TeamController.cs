﻿using Microsoft.AspNetCore.Mvc;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Controllers
{
    [Route("api/teams")]
    [ApiController]
    public class TeamController : ControllerBase
    {

        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }
        
        [HttpGet("{teamId}")]
        public async Task<ActionResult<ServiceResponse<TeamCompleteDto>>> GetTeamById(int teamId)
        {

            try
            {
                var result = await _teamService.GetTeamById(teamId);

                if (!result.Success)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (ArgumentException ex) 
            {
                return StatusCode(500, ex.Message); 
            }

        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<TeamSimpleDto>>>> GetAllTeams()
        {
            try
            {
                var result = await _teamService.GetAllTeams();

                if (!result.Success)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResponse<List<TeamSimpleDto>>
                {
                    Success = false,
                    Message = $"Ocorreu um erro ao buscar todos os times: {ex.Message}"
                });
            }
        }


        [HttpGet("players")]
        public async Task<ActionResult<ServiceResponse<List<TeamSimpleWithPlayersDto>>>> GetAllTeamsWithPlayers()
        {

            var result = await _teamService.GetAllTeamsWithPlayers();
            return Ok(result);

        }

        
        [HttpGet("GetTeamByLoggedUser")]
        public async Task<ActionResult<ServiceResponse<TeamCompleteDto>>> GetTeamByLoggedUser()
        {

            var result = await _teamService.GetTeamByLoggedUser();

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);

        }

        [HttpPut("{teamId}/gameplan")]
        public async Task<ActionResult> UpdateTeamGameplan(int teamId, TeamGameplanDto teamGameplanDto)
        {
            var response = await _teamService.UpdateTeamGameplan(teamId, teamGameplanDto);

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok();
        }

        [HttpPut("{teamId}/keys")]
        public async Task<ActionResult> UpdateKeyPlayers(int teamId, List<PlayerCompleteDto> players)
        {
            var response = await _teamService.UpdateKeyPlayers(teamId, players);

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok();
        }

    }
}
