using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Server.Features.Trades.Contracts;
using NBALigaSimulation.Shared.Dtos.Trades;
using NBALigaSimulation.Shared.Models.GameNews;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Teams;
using NBALigaSimulation.Shared.Models.Trades;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Features.Trades;

public class TradeCommands : ITradeCommands
{
    private readonly ITradeProposalValidator _tradeProposalValidator;
    private readonly IGenericRepository<Trade> _tradeRepository;
    private readonly IGenericRepository<Player> _playerRepository;
    private readonly IGenericRepository<Team> _teamRepository;
    private readonly IGenericRepository<TeamDraftPicks> _draftPickRepository;
    private readonly IGenericRepository<News> _newsRepository;
    private readonly IMapper _mapper;

    public TradeCommands(
        ITradeProposalValidator tradeProposalValidator,
        IGenericRepository<Trade> tradeRepository,
        IGenericRepository<Player> playerRepository,
        IGenericRepository<Team> teamRepository,
        IGenericRepository<TeamDraftPicks> draftPickRepository,
        IGenericRepository<News> newsRepository,
        IMapper mapper)
    {
        _tradeProposalValidator = tradeProposalValidator;
        _tradeRepository = tradeRepository;
        _playerRepository = playerRepository;
        _teamRepository = teamRepository;
        _draftPickRepository = draftPickRepository;
        _newsRepository = newsRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<TradeDto>> CreateTrade(TradeCreateDto tradeDto)
    {
        var response = new ServiceResponse<TradeDto>();

        var validation = await _tradeProposalValidator.ValidateCreateTrade(tradeDto);
        if (!validation.Success)
        {
            response.Success = false;
            response.Message = validation.Message;
            return response;
        }

        try
        {
            Trade trade = _mapper.Map<Trade>(tradeDto);
            trade.DateCreated = DateTime.UtcNow;
            trade.LastModified = DateTime.UtcNow;
            trade.TradePlayers ??= new List<TradePlayer>();
            trade.TradePicks ??= new List<TradePicks>();

            foreach (var player in tradeDto.Players ?? new List<Shared.Dtos.Players.PlayerCompleteDto>())
            {
                trade.TradePlayers.Add(new TradePlayer
                {
                    PlayerId = player.Id,
                    Trade = trade
                });
            }

            foreach (var picks in tradeDto.DraftPicks ?? new List<Shared.Dtos.Teams.TeamDraftPickDto>())
            {
                trade.TradePicks.Add(new TradePicks
                {
                    DraftPickId = picks.Id,
                    Trade = trade
                });
            }

            await _tradeRepository.AddAsync(trade);
            await _tradeRepository.SaveChangesAsync();
            response.Success = true;
            response.Data = _mapper.Map<TradeDto>(trade);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Ocorreu um erro ao criar a trade: " + ex.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<bool>> UpdateTrade(TradeDto dto)
    {
        ServiceResponse<bool> response = new ServiceResponse<bool>();

        Trade trade = await _tradeRepository.Query()
            .Include(t => t.TradePlayers)
            .Include(t => t.TradePicks)
            .FirstOrDefaultAsync(p => p.Id == dto.Id);

        var teamOne = await _teamRepository.Query()
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == dto.TeamOneId);
        var teamTwo = await _teamRepository.Query()
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == dto.TeamTwoId);

        if (trade == null)
        {
            response.Success = false;
            response.Message = "Trade não encontrado.";
            return response;
        }

        var playersToTeamOne = new List<Player>();
        var playersToTeamTwo = new List<Player>();
        var picksToTeamOne = new List<TeamDraftPicks>();
        var picksToTeamTwo = new List<TeamDraftPicks>();

        if (dto.Response == true)
        {
            trade.Response = true;

            foreach (var player in trade.TradePlayers)
            {
                int randomNumber = Random.Shared.Next(15, 1000);
                var playerDb = await _playerRepository.Query()
                    .FirstOrDefaultAsync(p => p.Id == player.PlayerId);

                if (playerDb == null)
                {
                    continue;
                }

                if (playerDb.TeamId == trade.TeamOneId)
                {
                    playersToTeamTwo.Add(playerDb);
                    playerDb.TeamId = trade.TeamTwoId;
                    playerDb.PtModifier = 1;
                    playerDb.RosterOrder = teamTwo?.Players.Count + randomNumber ?? randomNumber;
                }
                else if (playerDb.TeamId == trade.TeamTwoId)
                {
                    playersToTeamOne.Add(playerDb);
                    playerDb.TeamId = trade.TeamOneId;
                    playerDb.PtModifier = 1;
                    playerDb.RosterOrder = teamOne?.Players.Count + randomNumber ?? randomNumber;
                }
            }

            foreach (var pick in trade.TradePicks)
            {
                var pickDb = await _draftPickRepository.Query()
                    .FirstOrDefaultAsync(p => p.Id == pick.DraftPickId);

                if (pickDb == null)
                {
                    continue;
                }

                if (pickDb.TeamId == trade.TeamOneId)
                {
                    picksToTeamTwo.Add(pickDb);
                    pickDb.TeamId = trade.TeamTwoId;
                }
                else if (pickDb.TeamId == trade.TeamTwoId)
                {
                    picksToTeamOne.Add(pickDb);
                    pickDb.TeamId = trade.TeamOneId;
                }
            }
        }

        if (dto.Response == false)
        {
            trade.Response = false;
        }

        try
        {
            await _tradeRepository.SaveChangesAsync();

            if (teamOne != null && teamTwo != null)
            {
                await UpdateRosterOrderAfterTrade(teamOne.Id, teamTwo.Id);
            }

            if (dto.Response == true && teamOne != null && teamTwo != null)
            {
                var tradeNews = new News
                {
                    Type = NewsType.Trade,
                    Title = $"Trade aprovado: {teamOne.Name ?? "Time A"} e {teamTwo.Name ?? "Time B"} fecham acordo.",
                    Summary = BuildTradeSummary(teamOne, teamTwo, playersToTeamOne, playersToTeamTwo, picksToTeamOne, picksToTeamTwo),
                    LinkEntityType = "tradeoffer",
                    LinkEntityId = trade.Id
                };
                await _newsRepository.AddAsync(tradeNews);
                await _newsRepository.SaveChangesAsync();
            }

            response.Success = true;
            response.Data = true;
        }
        catch
        {
            response.Success = false;
            response.Message = "Erro ao atualizar o Trade.";
        }

        return response;
    }

    public async Task<ServiceResponse<bool>> DeleteTrade(int tradeId)
    {
        var response = new ServiceResponse<bool>();

        try
        {
            var trade = await _tradeRepository.GetByIdAsync(tradeId);

            if (trade == null)
            {
                response.Success = false;
                response.Message = "A trade não foi encontrada.";
                return response;
            }

            _tradeRepository.Remove(trade);
            await _tradeRepository.SaveChangesAsync();

            response.Success = true;
            response.Data = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Ocorreu um erro ao excluir a trade: " + ex.Message;
        }

        return response;
    }

    private async Task<ServiceResponse<bool>> UpdateRosterOrderAfterTrade(int teamOneId, int teamTwoId)
    {
        var response = new ServiceResponse<bool>();

        var teamOneList = await _playerRepository.Query()
            .OrderBy(p => p.RosterOrder)
            .Where(p => p.TeamId == teamOneId)
            .ToListAsync();
        var teamTwoList = await _playerRepository.Query()
            .OrderBy(p => p.RosterOrder)
            .Where(p => p.TeamId == teamTwoId)
            .ToListAsync();

        foreach (var player in teamOneList)
        {
            var playerDb = await _playerRepository.GetByIdAsync(player.Id);
            if (playerDb == null)
            {
                response.Success = false;
                response.Message = $"O Player com o Id {player.Id} não existe!";
                return response;
            }

            playerDb.RosterOrder = teamOneList.IndexOf(player);
        }

        foreach (var player in teamTwoList)
        {
            var playerDb = await _playerRepository.GetByIdAsync(player.Id);
            if (playerDb == null)
            {
                response.Success = false;
                response.Message = $"O Player com o Id {player.Id} não existe!";
                return response;
            }

            playerDb.RosterOrder = teamTwoList.IndexOf(player);
        }

        await _playerRepository.SaveChangesAsync();
        response.Success = true;
        response.Message = "Roster order updated successfully.";

        return response;
    }

    private static string BuildTradeSummary(
        Team teamOne,
        Team teamTwo,
        List<Player> playersToTeamOne,
        List<Player> playersToTeamTwo,
        List<TeamDraftPicks> picksToTeamOne,
        List<TeamDraftPicks> picksToTeamTwo)
    {
        string FormatPlayerList(List<Player> players) =>
            players.Count == 0 ? "nenhum jogador" : string.Join(", ", players.Select(p => p.Name));

        string FormatPickList(List<TeamDraftPicks> picks)
        {
            if (picks.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(", ", picks.Select(p =>
                $"{p.Year} {(p.Round == 1 ? "1ª rodada" : "2ª rodada")} ({p.Original})"));
        }

        var teamOneParts = new List<string>();
        var teamTwoParts = new List<string>();

        if (playersToTeamOne.Count > 0)
        {
            teamOneParts.Add(FormatPlayerList(playersToTeamOne));
        }

        if (picksToTeamOne.Count > 0)
        {
            teamOneParts.Add(FormatPickList(picksToTeamOne));
        }

        if (playersToTeamTwo.Count > 0)
        {
            teamTwoParts.Add(FormatPlayerList(playersToTeamTwo));
        }

        if (picksToTeamTwo.Count > 0)
        {
            teamTwoParts.Add(FormatPickList(picksToTeamTwo));
        }

        var teamOneReceives = teamOneParts.Count > 0 ? string.Join("; ", teamOneParts) : "nenhum ativo";
        var teamTwoReceives = teamTwoParts.Count > 0 ? string.Join("; ", teamTwoParts) : "nenhum ativo";

        return $"{teamOne.Name ?? "Time A"} recebe: {teamOneReceives}. {teamTwo.Name ?? "Time B"} recebe: {teamTwoReceives}.";
    }
}
