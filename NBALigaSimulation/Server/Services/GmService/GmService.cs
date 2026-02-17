using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Shared.Dtos.Gm;
using NBALigaSimulation.Shared.Models.Teams;
using NBALigaSimulation.Shared.Models.Users;
using NBALigaSimulation.Shared.Models.Utils;
using NBALigaSimulation.Server.Repositories.Interfaces;

namespace NBALigaSimulation.Server.Services.GmService;

public class GmService : IGmService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Team> _teamRepository;
    private readonly IAuthService _authService;

    public GmService(
        IGenericRepository<User> userRepository,
        IGenericRepository<Team> teamRepository,
        IAuthService authService)
    {
        _userRepository = userRepository;
        _teamRepository = teamRepository;
        _authService = authService;
    }

    public async Task<ServiceResponse<GmProfileDto>> GetMyProfile()
    {
        var response = new ServiceResponse<GmProfileDto>();
        try
        {
            var userId = _authService.GetUserId();
            var user = await _userRepository.Query()
                .Include(u => u.Team)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                response.Success = false;
                response.Message = "Usuário não encontrado.";
                return response;
            }

            if (user.TeamId == null || user.Team == null)
            {
                response.Data = new GmProfileDto
                {
                    UserId = user.Id,
                    Username = user.Username ?? "",
                    TeamId = null,
                    TeamName = "",
                    TeamAbrv = "",
                    TeamRegion = "",
                    Championships = 0,
                    TotalWins = 0,
                    TotalLosses = 0,
                    TotalGames = 0,
                    TotalWinPct = "0.000",
                    SeasonRecords = new List<GmSeasonRecordDto>()
                };
                response.Success = true;
                return response;
            }

            var team = await _teamRepository.Query()
                .Include(t => t.TeamRegularStats)
                .FirstOrDefaultAsync(t => t.Id == user.TeamId);

            if (team == null)
            {
                response.Success = false;
                response.Message = "Time não encontrado.";
                return response;
            }

            var stats = team.TeamRegularStats?.OrderByDescending(s => s.Season).ToList() ?? new List<TeamRegularStats>();
            int totalWins = stats.Sum(s => s.HomeWins + s.RoadWins);
            int totalLosses = stats.Sum(s => s.HomeLosses + s.RoadLosses);
            int totalGames = totalWins + totalLosses;
            double totalWinPct = totalGames > 0 ? (double)totalWins / totalGames : 0;

            var seasonRecords = stats.Select(s =>
            {
                int w = s.HomeWins + s.RoadWins;
                int l = s.HomeLosses + s.RoadLosses;
                int g = w + l;
                double pct = g > 0 ? (double)w / g : 0;
                return new GmSeasonRecordDto
                {
                    Season = s.Season,
                    Wins = w,
                    Losses = l,
                    Games = g,
                    WinPct = pct.ToString(".000", CultureInfo.InvariantCulture),
                    PlayoffWins = s.PlayoffWins,
                    PlayoffLosses = s.PlayoffLosses
                };
            }).ToList();

            response.Data = new GmProfileDto
            {
                UserId = user.Id,
                Username = user.Username ?? "",
                TeamId = team.Id,
                TeamName = team.Name ?? "",
                TeamAbrv = team.Abrv ?? "",
                TeamRegion = team.Region ?? "",
                Championships = team.Championships,
                TotalWins = totalWins,
                TotalLosses = totalLosses,
                TotalGames = totalGames,
                TotalWinPct = totalWinPct.ToString(".000", CultureInfo.InvariantCulture),
                SeasonRecords = seasonRecords
            };
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Erro ao obter perfil do GM: " + ex.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<GmProfileDto>> GetProfileByTeamId(int teamId)
    {
        var response = new ServiceResponse<GmProfileDto>();
        try
        {
            var user = await _userRepository.Query()
                .Include(u => u.Team)
                .FirstOrDefaultAsync(u => u.TeamId == teamId);

            if (user == null)
            {
                response.Success = false;
                response.Message = "Nenhum GM associado a este time.";
                return response;
            }

            var team = await _teamRepository.Query()
                .Include(t => t.TeamRegularStats)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
            {
                response.Success = false;
                response.Message = "Time não encontrado.";
                return response;
            }

            var stats = team.TeamRegularStats?.OrderByDescending(s => s.Season).ToList() ?? new List<TeamRegularStats>();
            int totalWins = stats.Sum(s => s.HomeWins + s.RoadWins);
            int totalLosses = stats.Sum(s => s.HomeLosses + s.RoadLosses);
            int totalGames = totalWins + totalLosses;
            double totalWinPct = totalGames > 0 ? (double)totalWins / totalGames : 0;

            var seasonRecords = stats.Select(s =>
            {
                int w = s.HomeWins + s.RoadWins;
                int l = s.HomeLosses + s.RoadLosses;
                int g = w + l;
                double pct = g > 0 ? (double)w / g : 0;
                return new GmSeasonRecordDto
                {
                    Season = s.Season,
                    Wins = w,
                    Losses = l,
                    Games = g,
                    WinPct = pct.ToString(".000", CultureInfo.InvariantCulture),
                    PlayoffWins = s.PlayoffWins,
                    PlayoffLosses = s.PlayoffLosses
                };
            }).ToList();

            response.Data = new GmProfileDto
            {
                UserId = user.Id,
                Username = user.Username ?? "",
                TeamId = team.Id,
                TeamName = team.Name ?? "",
                TeamAbrv = team.Abrv ?? "",
                TeamRegion = team.Region ?? "",
                Championships = team.Championships,
                TotalWins = totalWins,
                TotalLosses = totalLosses,
                TotalGames = totalGames,
                TotalWinPct = totalWinPct.ToString(".000", CultureInfo.InvariantCulture),
                SeasonRecords = seasonRecords
            };
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Erro ao obter perfil do GM: " + ex.Message;
        }

        return response;
    }
}
