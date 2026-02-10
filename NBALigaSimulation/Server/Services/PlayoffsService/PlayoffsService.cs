using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Shared.Dtos.Playoffs;
using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.SeasonPlayoffs;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Teams;
using NBALigaSimulation.Shared.Models.Utils;


namespace NBALigaSimulation.Server.Services.PlayoffsService
{
    public class PlayoffsService : IPlayoffsService
    {

        private readonly IGenericRepository<Season> _seasonRepository;
        private readonly IGenericRepository<Playoffs> _playoffsRepository;
        private readonly IGenericRepository<Game> _gameRepository;
        private readonly IGenericRepository<PlayoffsGame> _playoffsGameRepository;
        private readonly IGenericRepository<Team> _teamRepository;
        private readonly IGenericRepository<Player> _playerRepository;
        private readonly IGenericRepository<PlayerAwards> _playerAwardsRepository;
        private readonly IMapper _mapper;

        public PlayoffsService(
            IGenericRepository<Season> seasonRepository,
            IGenericRepository<Playoffs> playoffsRepository,
            IGenericRepository<Game> gameRepository,
            IGenericRepository<PlayoffsGame> playoffsGameRepository,
            IGenericRepository<Team> teamRepository,
            IGenericRepository<Player> playerRepository,
            IGenericRepository<PlayerAwards> playerAwardsRepository,
            IMapper mapper)
        {
            _seasonRepository = seasonRepository;
            _playoffsRepository = playoffsRepository;
            _gameRepository = gameRepository;
            _playoffsGameRepository = playoffsGameRepository;
            _teamRepository = teamRepository;
            _playerRepository = playerRepository;
            _playerAwardsRepository = playerAwardsRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<PlayoffsDto>>> GetPlayoffs()
        {
            var response = new ServiceResponse<List<PlayoffsDto>>();
            var season = await _seasonRepository.Query()
                .OrderBy(s => s.Year)
                .LastOrDefaultAsync();

            var playoffs = await _playoffsRepository.Query()
                .Where(p => p.Season == season.Year)
                .Include(p => p.TeamOne)
                .Include(p => p.TeamTwo)
                .ToListAsync();

            if (playoffs == null)
            {
                response.Success = false;
                response.Message = $"Playoffs não econtrado!";
            }
            else
            {

                response.Data = _mapper.Map<List<PlayoffsDto>>(playoffs);
            }

            return response;
        }

        public async Task<ServiceResponse<PlayoffsDto>> GetPlayoffsById(int Id)
        {
            var response = new ServiceResponse<PlayoffsDto>();
            var season = await _seasonRepository.Query()
                .OrderBy(s => s.Year)
                .LastOrDefaultAsync();

            var playoff = await _playoffsRepository.Query()
                .Where(p => p.Id == Id && p.Season == season.Year)
                .OrderBy(p => p.Id)
                .Include(p => p.TeamOne)
                .Include(p => p.TeamTwo)
                .Include(t => t.PlayoffGames)
                .ThenInclude(t => t.Game)
                .ThenInclude(g => g.TeamGameStats)
                .LastOrDefaultAsync();

            if (playoff == null)
            {
                response.Success = false;
                response.Message = $"Playoffs não econtrado!";
            }
            else
            {

                response.Data = _mapper.Map<PlayoffsDto>(playoff);
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> GeneratePlayoffs()
        {
            var response = new ServiceResponse<bool>();
            var season = await _seasonRepository.Query()
                .OrderBy(s => s.Year)
                .LastOrDefaultAsync();

            List<Team> teamsEast = await _teamRepository.Query()
                .Include(t => t.TeamRegularStats)
                .Where(t => t.Conference == "East" && t.TeamRegularStats.Any(trs => trs.Season == season.Year))
                .ToListAsync();

            List<Team> teamsWest = await _teamRepository.Query()
                .Include(t => t.TeamRegularStats)
                .Where(t => t.Conference == "West" && t.TeamRegularStats.Any(trs => trs.Season == season.Year))
                .ToListAsync();


            var playoffsExists = await _playoffsRepository.Query()
                .Where(p => p.Season == season.Year)
                .ToListAsync();

            if (playoffsExists.Count > 0)
            {
                response.Success = false;
                response.Message = $"Já existe playoffs da temporada {season.Year}!";
                return response;
            }

            var playoffs = PlayoffsUtils.Generate1stRound(teamsEast, teamsWest, season.Year);
            var games = PlayoffsUtils.GenerateRoundGames(playoffs, season);
            season.RegularCompleted = true;

            await _playoffsGameRepository.AddRangeAsync(games);
            await _playoffsRepository.AddRangeAsync(playoffs);
            await _playoffsRepository.SaveChangesAsync();

            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<bool>> Generate2Round()
        {
            var response = new ServiceResponse<bool>();
            var season = await _seasonRepository.Query()
                .OrderBy(s => s.Year)
                .LastOrDefaultAsync();

            List<Playoffs> playoffs = await _playoffsRepository.Query()
                .Where(p => p.Season == season.Year)
                .Include(t => t.TeamOne).ThenInclude(t => t.TeamRegularStats)
                .Include(t => t.TeamTwo).ThenInclude(t => t.TeamRegularStats)
                .ToListAsync();

            if (playoffs.Any(p => p.SeriesId == 9))
            {
                response.Success = false;
                response.Message = "Já existe um 2º round gerado!";
                return response;
            }

            var newPlayoffs = PlayoffsUtils.Generate2ndRound(playoffs, season.Year);
            var games = PlayoffsUtils.GenerateRoundGames(newPlayoffs, season);

            await _playoffsGameRepository.AddRangeAsync(games);
            await _playoffsRepository.AddRangeAsync(newPlayoffs);
            await _playoffsRepository.SaveChangesAsync();

            response.Message = "2º round gerado sucesso!";
            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<bool>> Generate3Round()
        {
            var response = new ServiceResponse<bool>();
            var season = await _seasonRepository.Query()
                .OrderBy(s => s.Year)
                .LastOrDefaultAsync();

            List<Playoffs> playoffs = await _playoffsRepository.Query()
                .Where(p => p.Season == season.Year)
                .Include(t => t.TeamOne).ThenInclude(t => t.TeamRegularStats)
                .Include(t => t.TeamTwo).ThenInclude(t => t.TeamRegularStats)
                .ToListAsync();

            if (playoffs.Any(p => p.SeriesId == 13))
            {
                response.Success = false;
                response.Message = "Já existe um 3º round gerado!";
                return response;
            }

            var newPlayoffs = PlayoffsUtils.Generate3ndRound(playoffs, season.Year);
            var games = PlayoffsUtils.GenerateRoundGames(newPlayoffs, season);

            await _playoffsGameRepository.AddRangeAsync(games);
            await _playoffsRepository.AddRangeAsync(newPlayoffs);
            await _playoffsRepository.SaveChangesAsync();

            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<bool>> Generate4Round()
        {
            var response = new ServiceResponse<bool>();
            var season = await _seasonRepository.Query()
                .OrderBy(s => s.Year)
                .LastOrDefaultAsync();

            List<Playoffs> playoffs = await _playoffsRepository.Query()
                .Where(p => p.Season == season.Year)
                .Include(t => t.TeamOne).ThenInclude(t => t.TeamRegularStats)
                .Include(t => t.TeamTwo).ThenInclude(t => t.TeamRegularStats)
                .ToListAsync();


            if (playoffs.Any(p => p.SeriesId == 15))
            {
                response.Success = false;
                response.Message = "Já existe um 4º round gerado!";
                return response;
            }

            var newPlayoffs = PlayoffsUtils.Generate4ndRound(playoffs, season.Year);
            var games = PlayoffsUtils.GenerateRoundGames(newPlayoffs, season);

            await _playoffsGameRepository.AddRangeAsync(games);
            await _playoffsRepository.AddRangeAsync(newPlayoffs);
            await _playoffsRepository.SaveChangesAsync();

            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<bool>> EndPlayoffs()
        {
            var response = new ServiceResponse<bool>();
            var season = await _seasonRepository.Query()
                .OrderBy(s => s.Year)
                .LastOrDefaultAsync();

            if (season.IsCompleted)
            {
                response.Success = false;
                response.Message = "A temporada já foi finalizada!";
                return response;
            }

            season.IsCompleted = true;

            var playoffs = await _playoffsRepository.Query()
                .Where(p => p.Season == season.Year && p.SeriesId == 15)
                .Include(g => g.PlayoffGames)
                .ToListAsync();

            if (playoffs.Count == 0)
            {
                response.Success = false;
                response.Message = "Não é possivel terminar o playoffs!";
                return response;
            }

            var championId = playoffs.FirstOrDefault(t => t.SeriesId == 15).WinsTeamOne == 4 ?
                playoffs.FirstOrDefault(t => t.SeriesId == 15).TeamOneId :
                playoffs.FirstOrDefault(t => t.SeriesId == 15).TeamTwoId;


            var championTeam = await _teamRepository.Query()
                .OrderBy(t => t.Id)
                .LastOrDefaultAsync(t => t.Id == championId);
            if (championTeam != null)
            {
                championTeam.Championships += 1;
            }

            var gamesList = playoffs.SelectMany(t => t.PlayoffGames).ToList();
            var gamesId = gamesList.Select(game => game.GameId).ToList();
            var games = await _gameRepository.Query()
                .Where(game => gamesId.Contains(game.Id))
                .Include(t => t.PlayerGameStats.Where(t => t.TeamId == championId))
                .ToListAsync();

            var playerGameScores = new Dictionary<int, double>();

            foreach (var game in games)
            {
                foreach (var playerGameStat in game.PlayerGameStats.Where(pgs => pgs.TeamId == championId))
                {
                    var playerId = playerGameStat.PlayerId;
                    var gameScore = playerGameStat.GameScore;
                    if (playerGameScores.ContainsKey(playerId))
                    {
                        playerGameScores[playerId] += gameScore;
                    }
                    else
                    {
                        playerGameScores[playerId] = gameScore;
                    }
                }
            }

            var playerIdWithMaxGameScore = playerGameScores.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
            var playerWithMaxGameScore = await _playerRepository.Query()
                .Include(p => p.PlayoffsStats)
                .Include(p => p.PlayerAwards)
                .SingleOrDefaultAsync(p => p.Id == playerIdWithMaxGameScore);

            if (playerWithMaxGameScore != null)
            {
                var playoffsStats = playerWithMaxGameScore.PlayoffsStats.SingleOrDefault(ps => ps.Season == season.Year);
                if (playoffsStats != null)
                {

                        var award = new PlayerAwards
                        {
                            PlayerId = playerIdWithMaxGameScore,
                            Player = playerWithMaxGameScore,
                            Award = "NBA Finals MVP",
                            Season = season.Year,
                            Team = championTeam.Name,
                            Ppg = playoffsStats.PtsPG,
                            Rpg = playoffsStats.TRebPG,
                            Apg = playoffsStats.AstPG,
                            Spg = playoffsStats.StlPG,
                            Bpg = playoffsStats.BlkPG
                        };

                        await _playerAwardsRepository.AddAsync(award);
                }
            }

            await _playerAwardsRepository.SaveChangesAsync();
            response.Success = true;
            return response;
        }

    }
}
