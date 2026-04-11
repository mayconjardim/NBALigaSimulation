using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Server.Features.Trades.Contracts;
using NBALigaSimulation.Shared.Dtos.Trades;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Features.Trades;

public class TradeProposalValidator : ITradeProposalValidator
{
    private readonly IGenericRepository<Team> _teamRepository;
    private readonly IGenericRepository<Player> _playerRepository;
    private readonly IGenericRepository<TeamDraftPicks> _draftPickRepository;

    public TradeProposalValidator(
        IGenericRepository<Team> teamRepository,
        IGenericRepository<Player> playerRepository,
        IGenericRepository<TeamDraftPicks> draftPickRepository)
    {
        _teamRepository = teamRepository;
        _playerRepository = playerRepository;
        _draftPickRepository = draftPickRepository;
    }

    public async Task<ServiceResponse<bool>> ValidateCreateTrade(TradeCreateDto tradeDto)
    {
        var response = new ServiceResponse<bool>();

        if (tradeDto == null)
        {
            response.Success = false;
            response.Message = "Proposta de trade inválida.";
            return response;
        }

        if (tradeDto.TeamOneId <= 0 || tradeDto.TeamTwoId <= 0)
        {
            response.Success = false;
            response.Message = "Os times envolvidos na trade são obrigatórios.";
            return response;
        }

        if (tradeDto.TeamOneId == tradeDto.TeamTwoId)
        {
            response.Success = false;
            response.Message = "Uma trade deve envolver dois times diferentes.";
            return response;
        }

        var teamIds = new[] { tradeDto.TeamOneId, tradeDto.TeamTwoId };
        var teamsFound = await _teamRepository.Query()
            .Where(t => teamIds.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync();

        if (teamsFound.Count != 2)
        {
            response.Success = false;
            response.Message = "Um ou mais times da proposta não existem.";
            return response;
        }

        var players = tradeDto.Players ?? new();
        var picks = tradeDto.DraftPicks ?? new();

        if (players.Count == 0 && picks.Count == 0)
        {
            response.Success = false;
            response.Message = "A proposta deve incluir ao menos um jogador ou pick.";
            return response;
        }

        if (players.Select(p => p.Id).Distinct().Count() != players.Count)
        {
            response.Success = false;
            response.Message = "A proposta possui jogadores duplicados.";
            return response;
        }

        if (picks.Select(p => p.Id).Distinct().Count() != picks.Count)
        {
            response.Success = false;
            response.Message = "A proposta possui picks duplicados.";
            return response;
        }

        if (players.Count > 0)
        {
            var playerIds = players.Select(p => p.Id).ToList();
            var playersDb = await _playerRepository.Query()
                .Where(p => playerIds.Contains(p.Id))
                .Select(p => new { p.Id, p.TeamId })
                .ToListAsync();

            if (playersDb.Count != playerIds.Count)
            {
                response.Success = false;
                response.Message = "Um ou mais jogadores da proposta não existem.";
                return response;
            }

            if (playersDb.Any(p => p.TeamId != tradeDto.TeamOneId && p.TeamId != tradeDto.TeamTwoId))
            {
                response.Success = false;
                response.Message = "A proposta contém jogador que não pertence aos times envolvidos.";
                return response;
            }

        }

        if (picks.Count > 0)
        {
            var pickIds = picks.Select(p => p.Id).ToList();
            var picksDb = await _draftPickRepository.Query()
                .Where(p => pickIds.Contains(p.Id))
                .Select(p => new { p.Id, p.TeamId })
                .ToListAsync();

            if (picksDb.Count != pickIds.Count)
            {
                response.Success = false;
                response.Message = "Um ou mais picks da proposta não existem.";
                return response;
            }

            if (picksDb.Any(p => p.TeamId != tradeDto.TeamOneId && p.TeamId != tradeDto.TeamTwoId))
            {
                response.Success = false;
                response.Message = "A proposta contém pick que não pertence aos times envolvidos.";
                return response;
            }
        }

        bool teamOneHasAssets =
            players.Any(p => p.TeamId == tradeDto.TeamOneId) ||
            picks.Any(p => p.TeamId == tradeDto.TeamOneId);
        bool teamTwoHasAssets =
            players.Any(p => p.TeamId == tradeDto.TeamTwoId) ||
            picks.Any(p => p.TeamId == tradeDto.TeamTwoId);

        if (!teamOneHasAssets || !teamTwoHasAssets)
        {
            response.Success = false;
            response.Message = "A proposta deve incluir ao menos um ativo de cada time.";
            return response;
        }

        response.Success = true;
        response.Data = true;
        return response;
    }
}
