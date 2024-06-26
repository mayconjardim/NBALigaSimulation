﻿using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.TeamService
{
    public class TeamService : ITeamService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public TeamService(DataContext context, IMapper mapper, IAuthService authService)
        {
            _context = context;
            _mapper = mapper;
            _authService = authService;
        }
        
        public async Task<ServiceResponse<TeamCompleteDto>> GetTeamById(int teamId)
        {
            var response = new ServiceResponse<TeamCompleteDto>();

            try
            {
                var team = await _context.Teams
                    .Include(t => t.Players)
                    .ThenInclude(p => p.Ratings)
                    .Include(t => t.Players)
                    .ThenInclude(p => p.Contract)
                    .Include(t => t.Players)
                    .ThenInclude(p => p.RegularStats)
                    .Include(t => t.DraftPicks)
                    .FirstOrDefaultAsync(t => t.Id == teamId);

                if (team == null)
                {
                    response.Success = false;
                    response.Message = $"O Time com o Id {teamId} não existe!";
                }
                else
                {
                    response.Data = _mapper.Map<TeamCompleteDto>(team);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Ocorreu um erro ao buscar o time com o ID {teamId}: {ex.Message}");
            }

            return response;
        }


        public async Task<ServiceResponse<List<TeamSimpleDto>>> GetAllTeams()
        {
            try
            {
                var teams = await _context.Teams
                    .Where(t => t.IsHuman)
                    .ToListAsync();

                var teamDtos = _mapper.Map<List<TeamSimpleDto>>(teams);

                return new ServiceResponse<List<TeamSimpleDto>>
                {
                    Data = teamDtos,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<TeamSimpleDto>>
                {
                    Success = false,
                    Message = $"Ocorreu um erro ao obter todos os times: {ex.Message}"
                };
            }
        }


        public async Task<ServiceResponse<List<TeamSimpleWithPlayersDto>>> GetAllTeamsWithPlayers()
        {
            var teams = await _context.Teams.Where(t => t.IsHuman == true).Include(t => t.Players).ThenInclude(p => p.Contract).ToListAsync();
            var response = new ServiceResponse<List<TeamSimpleWithPlayersDto>>
            {
                Data = _mapper.Map<List<TeamSimpleWithPlayersDto>>(teams)
            };

            return response;
        }

        [Authorize]
        public async Task<ServiceResponse<TeamCompleteDto>> GetTeamByLoggedUser()
        {

            var response = new ServiceResponse<TeamCompleteDto>();
            var userId = _authService.GetUserId();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                response.Success = false;
                response.Message = "Usuário não encontrado!";
                return response;
            }

            var teamId = user.TeamId;

            if (teamId == null)
            {
                response.Success = false;
                response.Message = "Usuário não está associado a um time!";
                return response;
            }

           var team = await _context.Teams
            .Include(t => t.Players.OrderBy(p => p.RosterOrder))
                .ThenInclude(p => p.Ratings)
            .Include(t => t.Gameplan)
            .Include(t => t.Players)
                .ThenInclude(p => p.RegularStats)
            .Include(t => t.DraftPicks)
            .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
            {
                response.Success = false;
                response.Message = $"O Time com o Id {teamId} não existe!";
                return response;
            }

            response.Data = _mapper.Map<TeamCompleteDto>(team);
            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateKeyPlayers(int teamId, List<PlayerCompleteDto> players)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            var team = await _context.Teams.Include(t => t.Players).FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
            {
                response.Message = $"O Time com o Id {teamId} não existe!";
                return response;
            }

            foreach (var player in players)
            {
                var existingPlayer = team.Players.FirstOrDefault(p => p.Id == player.Id);
                if (existingPlayer != null)
                {
                    existingPlayer.KeyPlayer = player.KeyPlayer;
                }
            }

            _context.Update(team);

            try
            {
                await _context.SaveChangesAsync();
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message = $"Erro ao atualizar o time: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateTeamGameplan(int teamId, TeamGameplanDto teamGameplanDto)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            var team = await _context.Teams.Include(t => t.Gameplan).FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
            {
                response.Message = $"O Time com o Id {teamId} não existe!";
                return response;
            }

            team.Gameplan.Pace = teamGameplanDto.Pace;
            team.Gameplan.Motion = teamGameplanDto.Motion;
            team.Gameplan.Focus = teamGameplanDto.Focus;
            team.Gameplan.Defense = teamGameplanDto.Defense;

            _context.Update(team);

            try
            {
                await _context.SaveChangesAsync();
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message = $"Error updating team gameplan: {ex.Message}";
            }

            return response;
        }

    }
}
