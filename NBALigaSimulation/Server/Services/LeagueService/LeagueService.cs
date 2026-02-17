using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Shared.Dtos.League;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.SeasonPlayoffs;
using NBALigaSimulation.Shared.Models.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.LeagueService
{
    public class LeagueService : ILeagueService
    {
        private readonly IGenericRepository<TeamDraftPicks> _draftPickRepository;
        private readonly IGenericRepository<Playoffs> _playoffsRepository;
        private readonly IGenericRepository<PlayerAwards> _playerAwardsRepository;
        private readonly IGenericRepository<TeamRegularStats> _teamRegularStatsRepository;
        private readonly IMapper _mapper;

        public LeagueService(
            IGenericRepository<TeamDraftPicks> draftPickRepository,
            IGenericRepository<Playoffs> playoffsRepository,
            IGenericRepository<PlayerAwards> playerAwardsRepository,
            IGenericRepository<TeamRegularStats> teamRegularStatsRepository,
            IMapper mapper)
        {
            _draftPickRepository = draftPickRepository;
            _playoffsRepository = playoffsRepository;
            _playerAwardsRepository = playerAwardsRepository;
            _teamRegularStatsRepository = teamRegularStatsRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<TeamDraftPickDto>>> GetAllDraftPicks()
        {
            var picks = await _draftPickRepository.Query()
                .Include(t => t.Team)
                .ToListAsync();
            var response = new ServiceResponse<List<TeamDraftPickDto>>
            {
                Data = _mapper.Map<List<TeamDraftPickDto>>(picks)
            };

            return response;
        }

        public async Task<ServiceResponse<List<SeasonHistoryDto>>> GetSeasonHistory()
        {
            var response = new ServiceResponse<List<SeasonHistoryDto>>();
            try
            {
                var finals = await _playoffsRepository.Query()
                    .Where(p => p.SeriesId == 15)
                    .Include(p => p.TeamOne)
                    .Include(p => p.TeamTwo)
                    .ToListAsync();

                var championBySeason = new Dictionary<int, (int TeamId, string Name, string Abrv)>();
                var runnerUpBySeason = new Dictionary<int, (int TeamId, string Name, string Abrv)>();
                foreach (var f in finals)
                {
                    if (f.WinsTeamOne >= 4)
                    {
                        championBySeason[f.Season] = (f.TeamOneId, f.TeamOne?.Name ?? "", f.TeamOne?.Abrv ?? "");
                        runnerUpBySeason[f.Season] = (f.TeamTwoId, f.TeamTwo?.Name ?? "", f.TeamTwo?.Abrv ?? "");
                    }
                    else if (f.WinsTeamTwo >= 4)
                    {
                        championBySeason[f.Season] = (f.TeamTwoId, f.TeamTwo?.Name ?? "", f.TeamTwo?.Abrv ?? "");
                        runnerUpBySeason[f.Season] = (f.TeamOneId, f.TeamOne?.Name ?? "", f.TeamOne?.Abrv ?? "");
                    }
                }

                var allAwards = await _playerAwardsRepository.Query()
                    .Include(a => a.Player)
                    .ToListAsync();
                var awardsBySeason = allAwards
                    .GroupBy(a => a.Season)
                    .ToDictionary(g => g.Key, g => g.Select(a => new SeasonAwardDto
                    {
                        AwardName = a.Award ?? "",
                        PlayerId = a.PlayerId,
                        PlayerName = a.Player?.Name ?? ""
                    }).ToList());

                var allSeasons = championBySeason.Keys.Union(runnerUpBySeason.Keys).Union(awardsBySeason.Keys).Distinct().OrderByDescending(y => y).ToList();

                var championTeamIdsBySeason = championBySeason.ToDictionary(k => k.Key, k => k.Value.TeamId);
                var runnerUpTeamIdsBySeason = runnerUpBySeason.ToDictionary(k => k.Key, k => k.Value.TeamId);
                var teamIdsForStats = new HashSet<(int Season, int TeamId)>();
                foreach (var kv in championTeamIdsBySeason) teamIdsForStats.Add((kv.Key, kv.Value));
                foreach (var kv in runnerUpTeamIdsBySeason) teamIdsForStats.Add((kv.Key, kv.Value));

                var teamIds = championTeamIdsBySeason.Values.Union(runnerUpTeamIdsBySeason.Values).Distinct().ToList();
                var allTeamStats = await _teamRegularStatsRepository.Query()
                    .Where(trs => allSeasons.Contains(trs.Season) && teamIds.Contains(trs.TeamId))
                    .ToListAsync();
                var statsBySeasonAndTeam = allTeamStats
                    .Where(trs => teamIdsForStats.Contains((trs.Season, trs.TeamId)))
                    .ToDictionary(trs => (trs.Season, trs.TeamId), trs => trs);

                var list = new List<SeasonHistoryDto>();
                foreach (var season in allSeasons)
                {
                    var hasChamp = championBySeason.TryGetValue(season, out var champ);
                    var hasRunnerUp = runnerUpBySeason.TryGetValue(season, out var runnerUp);
                    awardsBySeason.TryGetValue(season, out var awards);
                    if (awards == null) awards = new List<SeasonAwardDto>();

                    string champRegular = "", champPlayoff = "";
                    int? champRank = null;
                    if (hasChamp && statsBySeasonAndTeam.TryGetValue((season, champ.TeamId), out var cStats))
                    {
                        champRegular = $"{cStats.HomeWins + cStats.RoadWins}-{cStats.HomeLosses + cStats.RoadLosses}";
                        champPlayoff = cStats.PlayoffWins > 0 || cStats.PlayoffLosses > 0 ? $"{cStats.PlayoffWins}-{cStats.PlayoffLosses}" : "";
                        champRank = cStats.ConfRank;
                    }

                    string ruRegular = "", ruPlayoff = "";
                    int? ruRank = null;
                    if (hasRunnerUp && statsBySeasonAndTeam.TryGetValue((season, runnerUp.TeamId), out var ruStats))
                    {
                        ruRegular = $"{ruStats.HomeWins + ruStats.RoadWins}-{ruStats.HomeLosses + ruStats.RoadLosses}";
                        ruPlayoff = ruStats.PlayoffWins > 0 || ruStats.PlayoffLosses > 0 ? $"{ruStats.PlayoffWins}-{ruStats.PlayoffLosses}" : "";
                        ruRank = ruStats.ConfRank;
                    }

                    list.Add(new SeasonHistoryDto
                    {
                        Season = season,
                        ChampionTeamId = hasChamp ? champ.TeamId : null,
                        ChampionTeamName = hasChamp ? champ.Name : "",
                        ChampionTeamAbrv = hasChamp ? champ.Abrv : "",
                        ChampionRegularRecord = champRegular,
                        ChampionPlayoffRecord = champPlayoff,
                        ChampionConfRank = champRank,
                        RunnerUpTeamId = hasRunnerUp ? runnerUp.TeamId : null,
                        RunnerUpTeamName = hasRunnerUp ? runnerUp.Name : "",
                        RunnerUpTeamAbrv = hasRunnerUp ? runnerUp.Abrv : "",
                        RunnerUpRegularRecord = ruRegular,
                        RunnerUpPlayoffRecord = ruPlayoff,
                        RunnerUpConfRank = ruRank,
                        Awards = awards
                    });
                }

                response.Data = list;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Erro ao obter histórico: " + ex.Message;
            }

            return response;
        }
    }
}
