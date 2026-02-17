using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.AwardsService
{
    public class AwardsService : IAwardsService
    {
        private readonly IGenericRepository<Season> _seasonRepository;
        private readonly IGenericRepository<Player> _playerRepository;
        private readonly IGenericRepository<Team> _teamRepository;
        private readonly IGenericRepository<PlayerAwards> _playerAwardsRepository;
        private readonly IGenericRepository<Game> _gameRepository;
        private readonly IMapper _mapper;

        public AwardsService(
            IGenericRepository<Season> seasonRepository,
            IGenericRepository<Player> playerRepository,
            IGenericRepository<Team> teamRepository,
            IGenericRepository<PlayerAwards> playerAwardsRepository,
            IGenericRepository<Game> gameRepository,
            IMapper mapper)
        {
            _seasonRepository = seasonRepository;
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
            _playerAwardsRepository = playerAwardsRepository;
            _gameRepository = gameRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<bool>> GenerateAwards(int season)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                // Verificar se há jogos da temporada regular não simulados
                var seasonEntity = await _seasonRepository.Query()
                    .Include(s => s.Games)
                    .FirstOrDefaultAsync(s => s.Year == season);

                if (seasonEntity == null)
                {
                    response.Success = false;
                    response.Message = $"Temporada {season} não encontrada.";
                    return response;
                }

                // Verificar se há jogos da temporada regular (Type == 0) não simulados
                var unsimulatedRegularGames = await _gameRepository.Query()
                    .Where(g => g.SeasonId == seasonEntity.Id && g.Type == 0 && !g.Happened)
                    .CountAsync();

                if (unsimulatedRegularGames > 0)
                {
                    response.Success = false;
                    response.Message = $"Não é possível gerar awards. Ainda existem {unsimulatedRegularGames} jogos da temporada regular não simulados.";
                    return response;
                }
                // Buscar todos os jogadores com suas estatísticas da temporada
                var players = await _playerRepository.Query()
                    .Include(p => p.RegularStats.Where(s => s.Season == season))
                    .Include(p => p.Team)
                    .ThenInclude(t => t.TeamRegularStats.Where(s => s.Season == season))
                    .Include(p => p.AwardCounts)
                    .ToListAsync();

                // Buscar todas as estatísticas de times para calcular rankings defensivos
                var allTeamStats = await _teamRepository.Query()
                    .Include(t => t.TeamRegularStats.Where(s => s.Season == season))
                    .ToListAsync();

                var teamStatsList = allTeamStats
                    .SelectMany(t => t.TeamRegularStats.Where(s => s.Season == season))
                    .ToList();

                // Calcular defensive rating rank para cada time
                var defensiveRatings = teamStatsList
                    .Select(ts => new
                    {
                        TeamId = ts.TeamId,
                        DefRating = CalculateDefensiveRating(ts),
                        TeamStats = ts
                    })
                    .OrderBy(x => x.DefRating)
                    .ToList();

                var defensiveRankDict = new Dictionary<int, int>();
                for (int i = 0; i < defensiveRatings.Count; i++)
                {
                    defensiveRankDict[defensiveRatings[i].TeamId] = i + 1;
                }

                // Filtrar jogadores que jogaram na temporada
                var eligiblePlayers = players
                    .Where(p => p.RegularStats.Any(s => s.Season == season && s.Games > 0))
                    .ToList();

                // Calcular MVP
                var mvpWinner = await CalculateMVP(eligiblePlayers, season, teamStatsList);
                if (mvpWinner != null)
                {
                    await AwardPlayer(mvpWinner, "MVP", season);
                }

                // Calcular DPOY
                var dpoyWinner = await CalculateDPOY(eligiblePlayers, season, defensiveRankDict);
                if (dpoyWinner != null)
                {
                    await AwardPlayer(dpoyWinner, "DPOY", season);
                }

                // Calcular Sixth Man of the Year
                var sixthManWinner = await CalculateSixthMan(eligiblePlayers, season, teamStatsList);
                if (sixthManWinner != null)
                {
                    await AwardPlayer(sixthManWinner, "Sixth Man of the Year", season);
                }

                await _playerAwardsRepository.SaveChangesAsync();
                await _playerRepository.SaveChangesAsync();

                response.Success = true;
                response.Data = true;
                response.Message = "Awards gerados com sucesso!";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro ao gerar awards: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<PlayerAwardsDto>>> GetAllAwards()
        {
            var response = new ServiceResponse<List<PlayerAwardsDto>>();

            try
            {
                var awards = await _playerAwardsRepository.Query()
                    .Include(a => a.Player)
                    .OrderByDescending(a => a.Season)
                    .ThenBy(a => a.Award)
                    .ToListAsync();

                response.Data = _mapper.Map<List<PlayerAwardsDto>>(awards);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro ao buscar awards: {ex.Message}";
            }

            return response;
        }

        private async Task<Player?> CalculateMVP(List<Player> players, int season, List<TeamRegularStats> teamStatsList)
        {
            var mvpScores = new List<(Player Player, double Score)>();

            foreach (var player in players)
            {
                var stats = player.RegularStats.FirstOrDefault(s => s.Season == season);
                if (stats == null || stats.Games == 0) continue;

                // Verificar se jogou pelo menos 75% da temporada (assumindo 82 jogos)
                int minGamesRequired = (int)(82 * 0.75);
                if (stats.Games < minGamesRequired) continue;

                // Calcular médias por jogo
                double pts = (double)stats.Pts / stats.Games;
                double reb = (double)stats.Trb / stats.Games;
                double ast = (double)stats.Ast / stats.Games;
                double stl = (double)stats.Stl / stats.Games;
                double blk = (double)stats.Blk / stats.Games;
                double tov = (double)stats.Tov / stats.Games;

                // Calcular TS%
                double tsPct = CalculateTrueShooting(stats);

                // Buscar seed do time
                var teamStats = teamStatsList.FirstOrDefault(ts => ts.TeamId == player.TeamId);
                int seed = teamStats?.ConfRank ?? 99;

                // Calcular MVP Score
                double mvpScore = (pts * 1.0)
                    + (reb * 1.2)
                    + (ast * 1.5)
                    + (stl * 2.5)
                    + (blk * 2.0)
                    - (tov * 2.0);

                // Team Wins Bonus
                if (seed == 1) mvpScore += 15;
                else if (seed == 2) mvpScore += 10;
                else if (seed == 3) mvpScore += 7;
                else if (seed > 6) mvpScore -= 10;

                // Efficiency Bonus
                if (tsPct > 62) mvpScore += 8;
                else if (tsPct < 52) mvpScore -= 5;

                // Games Played Bonus (já filtrado acima, mas pode adicionar bônus extra)
                if (stats.Games >= minGamesRequired) mvpScore += 2;

                mvpScores.Add((player, mvpScore));
            }

            var winner = mvpScores.OrderByDescending(x => x.Score).FirstOrDefault();
            return winner.Player;
        }

        private async Task<Player?> CalculateDPOY(List<Player> players, int season, Dictionary<int, int> defensiveRankDict)
        {
            var dpoyScores = new List<(Player Player, double Score)>();

            foreach (var player in players)
            {
                var stats = player.RegularStats.FirstOrDefault(s => s.Season == season);
                if (stats == null || stats.Games == 0) continue;

                // Calcular médias por jogo
                double stl = (double)stats.Stl / stats.Games;
                double blk = (double)stats.Blk / stats.Games;
                double drb = (double)stats.Drb / stats.Games;

                // Calcular DPOY Score
                double dpoyScore = (stl * 3.0)
                    + (blk * 3.5)
                    + (drb * 1.0);

                // Defensive Team Bonus
                int defRank = defensiveRankDict.GetValueOrDefault(player.TeamId, 99);
                if (defRank <= 3) dpoyScore += 12;
                else if (defRank <= 10) dpoyScore += 5;
                else dpoyScore -= 5;

                // Rim Protection Bonus (para pivôs - C ou PF)
                if ((player.Pos == "C" || player.Pos == "PF") && blk >= 2.5)
                {
                    dpoyScore += 8;
                }

                dpoyScores.Add((player, dpoyScore));
            }

            var winner = dpoyScores.OrderByDescending(x => x.Score).FirstOrDefault();
            return winner.Player;
        }

        private async Task<Player?> CalculateSixthMan(List<Player> players, int season, List<TeamRegularStats> teamStatsList)
        {
            var sixthManScores = new List<(Player Player, double Score)>();

            // Calcular ranking de pontos do banco por time
            var benchPointsByTeam = players
                .Where(p => p.RegularStats.Any(s => s.Season == season))
                .GroupBy(p => p.TeamId)
                .Select(g => new
                {
                    TeamId = g.Key,
                    BenchPoints = g.Where(p =>
                    {
                        var s = p.RegularStats.FirstOrDefault(st => st.Season == season);
                        return s != null && s.Gs <= 25; // Não é starter
                    }).Sum(p =>
                    {
                        var s = p.RegularStats.FirstOrDefault(st => st.Season == season);
                        return s?.Pts ?? 0;
                    })
                })
                .OrderByDescending(x => x.BenchPoints)
                .ToList();

            var benchRankDict = new Dictionary<int, int>();
            for (int i = 0; i < benchPointsByTeam.Count; i++)
            {
                benchRankDict[benchPointsByTeam[i].TeamId] = i + 1;
            }

            foreach (var player in players)
            {
                var stats = player.RegularStats.FirstOrDefault(s => s.Season == season);
                if (stats == null || stats.Games == 0) continue;

                // Starter Penalty - desqualificar se começou mais de 25 jogos
                if (stats.Gs > 25) continue;

                // Calcular médias por jogo
                double pts = (double)stats.Pts / stats.Games;
                double ast = (double)stats.Ast / stats.Games;
                double reb = (double)stats.Trb / stats.Games;

                // Calcular 6MAN Score
                double sixthManScore = (pts * 1.3)
                    + (ast * 1.0)
                    + (reb * 0.7);

                // Bench Impact Bonus
                int benchRank = benchRankDict.GetValueOrDefault(player.TeamId, 99);
                if (benchRank <= 5) sixthManScore += 10;

                sixthManScores.Add((player, sixthManScore));
            }

            var winner = sixthManScores.OrderByDescending(x => x.Score).FirstOrDefault();
            return winner.Player;
        }

        private double CalculateTrueShooting(PlayerRegularStats stats)
        {
            if (stats.Fga == 0 && stats.Fta == 0) return 0;
            double ts = (double)stats.Pts / (2.0 * (stats.Fga + (0.44 * stats.Fta))) * 100;
            return ts;
        }

        private double CalculateDefensiveRating(TeamRegularStats teamStats)
        {
            // Defensive Rating = pontos permitidos por jogo (simplificado)
            // Quanto menor, melhor
            int games = teamStats.Games;
            if (games == 0) return 999;
            return (double)teamStats.AllowedPoints / games;
        }

        private async Task AwardPlayer(Player player, string awardName, int season)
        {
            var stats = player.RegularStats.FirstOrDefault(s => s.Season == season);
            if (stats == null) return;

            // Criar registro em PlayerAwards
            var award = new PlayerAwards
            {
                PlayerId = player.Id,
                Player = player,
                Award = awardName,
                Season = season,
                Team = player.Team?.Name ?? "",
                Ppg = ((double)stats.Pts / stats.Games).ToString("0.0"),
                Rpg = ((double)stats.Trb / stats.Games).ToString("0.0"),
                Apg = ((double)stats.Ast / stats.Games).ToString("0.0"),
                Spg = ((double)stats.Stl / stats.Games).ToString("0.0"),
                Bpg = ((double)stats.Blk / stats.Games).ToString("0.0")
            };

            await _playerAwardsRepository.AddAsync(award);

            // Atualizar PlayerAwardCounts
            if (player.AwardCounts == null)
            {
                player.AwardCounts = new PlayerAwardCounts
                {
                    PlayerId = player.Id,
                    Player = player
                };
            }

            switch (awardName)
            {
                case "MVP":
                    player.AwardCounts.MVP += 1;
                    break;
                case "DPOY":
                    player.AwardCounts.DPOY += 1;
                    break;
                case "Sixth Man of the Year":
                    player.AwardCounts.SixthManOfTheYear += 1;
                    break;
            }
        }
    }
}
