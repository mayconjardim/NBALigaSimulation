using AutoMapper;
using NBALigaSimulation.Shared.Engine.Utils;

namespace NBALigaSimulation.Server.Services.PlayoffsService
{
    public class PlayoffsService : IPlayoffsService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public PlayoffsService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<PlayoffsDto>>> GetPlayoffs()
        {
            var response = new ServiceResponse<List<PlayoffsDto>>();
            var season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();

            var playoffs = await _context.Playoffs.Where(p => p.Season == season.Year)
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
            var season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();

            var playoff = await _context.Playoffs.Where(p => p.Id == Id && p.Season == season.Year)
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
            var season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();

            List<Team> teamsEast = await _context.Teams
                .Include(t => t.TeamRegularStats)
                .Where(t => t.Conference == "East" && t.TeamRegularStats.Any(trs => trs.Season == season.Year))
                .ToListAsync();

            List<Team> teamsWest = await _context.Teams
                .Include(t => t.TeamRegularStats)
                .Where(t => t.Conference == "West" && t.TeamRegularStats.Any(trs => trs.Season == season.Year))
                .ToListAsync();

            var playoffs = PlayoffsUtils.Generate1stRound(teamsEast, teamsWest, season.Year);
            var games = PlayoffsUtils.GenerateRoundGames(playoffs, season);

            _context.AddRange(games);
            _context.AddRange(playoffs);
            await _context.SaveChangesAsync();

            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<bool>> Generate2Round()
        {
            var response = new ServiceResponse<bool>();
            var season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();

            List<Playoffs> playoffs = await _context.Playoffs
            .Where(p => p.Season == season.Year)
            .Include(t => t.TeamOne).ThenInclude(t => t.TeamRegularStats)
            .Include(t => t.TeamTwo).ThenInclude(t => t.TeamRegularStats)
            .ToListAsync();

            var newPlayoffs = PlayoffsUtils.Generate2ndRound(playoffs, season.Year);
            var games = PlayoffsUtils.GenerateRoundGames(newPlayoffs, season);

            _context.AddRange(games);
            _context.AddRange(newPlayoffs);
            await _context.SaveChangesAsync();

            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<bool>> Generate3Round()
        {
            var response = new ServiceResponse<bool>();
            var season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();

            List<Playoffs> playoffs = await _context.Playoffs
            .Where(p => p.Season == season.Year)
            .Include(t => t.TeamOne).ThenInclude(t => t.TeamRegularStats)
            .Include(t => t.TeamTwo).ThenInclude(t => t.TeamRegularStats)
            .ToListAsync();

            var newPlayoffs = PlayoffsUtils.Generate3ndRound(playoffs, season.Year);
            var games = PlayoffsUtils.GenerateRoundGames(newPlayoffs, season);

            _context.AddRange(games);
            _context.AddRange(newPlayoffs);
            await _context.SaveChangesAsync();

            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<bool>> Generate4Round()
        {
            var response = new ServiceResponse<bool>();
            var season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();

            List<Playoffs> playoffs = await _context.Playoffs
            .Where(p => p.Season == season.Year)
            .Include(t => t.TeamOne).ThenInclude(t => t.TeamRegularStats)
            .Include(t => t.TeamTwo).ThenInclude(t => t.TeamRegularStats)
            .ToListAsync();

            var newPlayoffs = PlayoffsUtils.Generate4ndRound(playoffs, season.Year);
            var games = PlayoffsUtils.GenerateRoundGames(newPlayoffs, season);

            _context.AddRange(games);
            _context.AddRange(newPlayoffs);
            await _context.SaveChangesAsync();

            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<bool>> EndPlayoffs()
        {
            var response = new ServiceResponse<bool>();
            var season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();

            var playoffs = await _context.Playoffs
                .Where(p => p.Season == season.Year && p.SeriesId == 15)
                .Include(g => g.PlayoffGames)
                .ToListAsync();

            var championId = playoffs.FirstOrDefault(t => t.SeriesId == 15).WinsTeamOne == 4 ?
                playoffs.FirstOrDefault(t => t.SeriesId == 15).TeamOneId :
                playoffs.FirstOrDefault(t => t.SeriesId == 15).TeamTwoId;

            var championTeam = _context.Teams.OrderBy(t => t.Id).LastOrDefault(t => t.Id == championId);
            if (championTeam != null)
            {
                championTeam.Championships += 1;
            }

            var gamesList = playoffs.SelectMany(t => t.PlayoffGames).ToList();
            var gamesId = gamesList.Select(game => game.GameId).ToList();
            var games = await _context.Games.Where(game => gamesId.Contains(game.Id)).Include(t => t.PlayerGameStats.Where(t => t.TeamId == championId)).ToListAsync();

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
            var playerWithMaxGameScore = await _context.Players.Include(p => p.PlayoffsStats).Include(p => p.PlayerAwards).SingleOrDefaultAsync(p => p.Id == playerIdWithMaxGameScore);

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

                    _context.Add(award);
                }
            }

            await _context.SaveChangesAsync();
            response.Success = true;
            return response;
        }

    }
}
