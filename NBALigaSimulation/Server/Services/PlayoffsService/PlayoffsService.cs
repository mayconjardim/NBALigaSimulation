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

            if (season == null)
            {
                response.Success = false;
                response.Message = "Temporada não encontrada.";
                return response;
            }

            // Carrega séries com jogos e estatísticas (Type=1 é playoffs)
            var playoffs = await _playoffsRepository.Query()
                .Where(p => p.Season == season.Year)
                .Include(p => p.TeamOne)
                .Include(p => p.TeamTwo)
                .Include(p => p.PlayoffGames)
                .ThenInclude(pg => pg.Game)
                .ThenInclude(g => g.TeamGameStats)
                .ToListAsync();

            if (playoffs == null || playoffs.Count == 0)
            {
                response.Success = true;
                response.Data = new List<PlayoffsDto>();
                return response;
            }

            response.Data = _mapper.Map<List<PlayoffsDto>>(playoffs);

            // Preenche WinsTeamOne/WinsTeamTwo e Complete a partir dos jogos já salvos (só para exibição)
            foreach (var dto in response.Data)
            {
                var serie = playoffs.FirstOrDefault(p => p.Id == dto.Id);
                if (serie?.PlayoffGames == null) continue;

                int winsOne = 0, winsTwo = 0;
                foreach (var pg in serie.PlayoffGames.Where(pg => pg.Game != null && pg.Game.Happened))
                {
                    var g = pg.Game;
                    var ptsOne = g.TeamGameStats?.Where(s => s.TeamId == serie.TeamOneId).Sum(s => s.Pts) ?? 0;
                    var ptsTwo = g.TeamGameStats?.Where(s => s.TeamId == serie.TeamTwoId).Sum(s => s.Pts) ?? 0;
                    if (ptsOne > ptsTwo) winsOne++;
                    else if (ptsTwo > ptsOne) winsTwo++;
                }
                dto.WinsTeamOne = winsOne;
                dto.WinsTeamTwo = winsTwo;
                dto.Complete = winsOne >= 4 || winsTwo >= 4;
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

            Console.WriteLine($"[GeneratePlayoffs] Temporada={season.Year}, Times Leste={teamsEast.Count}, Times Oeste={teamsWest.Count}");

            var playoffsExists = await _playoffsRepository.Query()
                .Where(p => p.Season == season.Year)
                .ToListAsync();

            if (playoffsExists.Count > 0)
            {
                response.Success = false;
                response.Message = $"Já existe playoffs da temporada {season.Year}!";
                Console.WriteLine($"[GeneratePlayoffs] ABORTADO: {response.Message}");
                return response;
            }

            var playoffs = PlayoffsUtils.Generate1stRound(teamsEast, teamsWest, season.Year);
            Console.WriteLine($"[GeneratePlayoffs] Séries de 1ª rodada geradas: {playoffs.Count}");
            season.RegularCompleted = true;

            // 1) Persistir Playoffs primeiro para obter Ids
            await _playoffsRepository.AddRangeAsync(playoffs);
            await _playoffsRepository.SaveChangesAsync();

            // 2) Gerar jogos da rodada (cada PlayoffsGame tem um Game novo)
            var playoffGames = PlayoffsUtils.GenerateRoundGames(playoffs, season);
            Console.WriteLine($"[GeneratePlayoffs] Jogos de playoffs (PlayoffsGame) criados: {playoffGames.Count}");

            // 3) Persistir os Games explicitamente e obter Ids (schedule dos playoffs)
            var gamesToInsert = playoffGames.Select(pg => pg.Game).ToList();
            foreach (var g in gamesToInsert)
                g.SeasonId = season.Id;
            await _gameRepository.AddRangeAsync(gamesToInsert);
            await _gameRepository.SaveChangesAsync();
            Console.WriteLine($"[GeneratePlayoffs] Jogos inseridos em Games: {gamesToInsert.Count}");

            // 4) Persistir o vínculo série-jogo (PlayoffsGame)
            await _playoffsGameRepository.AddRangeAsync(playoffGames);
            await _playoffsGameRepository.SaveChangesAsync();

            Console.WriteLine($"[GeneratePlayoffs] PlayoffsGame inseridos: {playoffGames.Count}");

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

            Console.WriteLine($"[Generate2Round] Temporada={season.Year}, séries atuais={playoffs.Count}");

            if (playoffs.Any(p => p.SeriesId == 9))
            {
                response.Success = false;
                response.Message = "Já existe um 2º round gerado!";
                Console.WriteLine($"[Generate2Round] ABORTADO: {response.Message}");
                return response;
            }

            List<Playoffs> newPlayoffs;
            try
            {
                newPlayoffs = PlayoffsUtils.Generate2ndRound(playoffs, season.Year);
                Console.WriteLine($"[Generate2Round] Novas séries (SeriesId 9-12): {newPlayoffs.Count}");
            }
            catch (InvalidOperationException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                Console.WriteLine($"[Generate2Round] ERRO: {ex.Message}");
                return response;
            }

            try
            {
                await _playoffsRepository.AddRangeAsync(newPlayoffs);
                await _playoffsRepository.SaveChangesAsync();
                Console.WriteLine($"[Generate2Round] 4 séries (9-12) inseridas no banco. Season={season.Year}.");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro ao salvar séries: {ex.Message}";
                Console.WriteLine($"[Generate2Round] ERRO ao salvar: {ex}");
                return response;
            }

            var playoffGames = PlayoffsUtils.GenerateRoundGames(newPlayoffs, season);
            var gamesToInsert = playoffGames.Select(pg => pg.Game).ToList();
            foreach (var g in gamesToInsert)
                g.SeasonId = season.Id;
            await _gameRepository.AddRangeAsync(gamesToInsert);
            await _gameRepository.SaveChangesAsync();

            Console.WriteLine($"[Generate2Round] Jogos inseridos em Games: {gamesToInsert.Count}, PlayoffsGame: {playoffGames.Count}");

            await _playoffsGameRepository.AddRangeAsync(playoffGames);
            await _playoffsGameRepository.SaveChangesAsync();

            response.Message = "2º round gerado sucesso!";
            response.Success = true;
            Console.WriteLine($"[Generate2Round] SUCESSO completo.");
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

            Console.WriteLine($"[Generate3Round] Temporada={season.Year}, séries atuais={playoffs.Count}");

            if (playoffs.Any(p => p.SeriesId == 13))
            {
                response.Success = false;
                response.Message = "Já existe um 3º round gerado!";
                Console.WriteLine($"[Generate3Round] ABORTADO: {response.Message}");
                return response;
            }

            List<Playoffs> newPlayoffs;
            try
            {
                newPlayoffs = PlayoffsUtils.Generate3ndRound(playoffs, season.Year);
                Console.WriteLine($"[Generate3Round] Novas séries (SeriesId 13-14): {newPlayoffs.Count}");
            }
            catch (InvalidOperationException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }

            await _playoffsRepository.AddRangeAsync(newPlayoffs);
            await _playoffsRepository.SaveChangesAsync();

            var playoffGames = PlayoffsUtils.GenerateRoundGames(newPlayoffs, season);
            var gamesToInsert = playoffGames.Select(pg => pg.Game).ToList();
            foreach (var g in gamesToInsert)
                g.SeasonId = season.Id;
            await _gameRepository.AddRangeAsync(gamesToInsert);
            await _gameRepository.SaveChangesAsync();

            await _playoffsGameRepository.AddRangeAsync(playoffGames);
            await _playoffsGameRepository.SaveChangesAsync();

            Console.WriteLine($"[Generate3Round] Jogos inseridos em Games: {gamesToInsert.Count}, PlayoffsGame: {playoffGames.Count}");

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
                Console.WriteLine($"[Generate4Round] ABORTADO: {response.Message}");
                return response;
            }

            List<Playoffs> newPlayoffs;
            try
            {
                newPlayoffs = PlayoffsUtils.Generate4ndRound(playoffs, season.Year);
                Console.WriteLine($"[Generate4Round] Novas séries (SeriesId 15): {newPlayoffs.Count}");
            }
            catch (InvalidOperationException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }

            await _playoffsRepository.AddRangeAsync(newPlayoffs);
            await _playoffsRepository.SaveChangesAsync();

            var playoffGames = PlayoffsUtils.GenerateRoundGames(newPlayoffs, season);
            var gamesToInsert = playoffGames.Select(pg => pg.Game).ToList();
            foreach (var g in gamesToInsert)
                g.SeasonId = season.Id;
            await _gameRepository.AddRangeAsync(gamesToInsert);
            await _gameRepository.SaveChangesAsync();

            await _playoffsGameRepository.AddRangeAsync(playoffGames);
            await _playoffsGameRepository.SaveChangesAsync();

            Console.WriteLine($"[Generate4Round] Jogos inseridos em Games: {gamesToInsert.Count}, PlayoffsGame: {playoffGames.Count}");

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
                .Include(t => t.Players)
                .ThenInclude(p => p.AwardCounts)
                .OrderBy(t => t.Id)
                .LastOrDefaultAsync(t => t.Id == championId);
            if (championTeam != null)
            {
                championTeam.Championships += 1;
                
                // Atualizar TitlesWon para todos os jogadores do time campeão
                foreach (var player in championTeam.Players)
                {
                    if (player.AwardCounts == null)
                    {
                        player.AwardCounts = new PlayerAwardCounts
                        {
                            PlayerId = player.Id,
                            Player = player
                        };
                    }
                    player.AwardCounts.TitlesWon += 1;
                }
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
                .Include(p => p.AwardCounts)
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

                        // Atualizar PlayerAwardCounts
                        if (playerWithMaxGameScore.AwardCounts == null)
                        {
                            playerWithMaxGameScore.AwardCounts = new PlayerAwardCounts
                            {
                                PlayerId = playerWithMaxGameScore.Id,
                                Player = playerWithMaxGameScore
                            };
                        }
                        // Nota: Não há campo específico para Finals MVP em AwardCounts, mas podemos adicionar depois se necessário
                }
            }

            await _playerAwardsRepository.SaveChangesAsync();
            await _playerRepository.SaveChangesAsync();
            await _teamRepository.SaveChangesAsync();
            response.Success = true;
            return response;
        }

    }
}
