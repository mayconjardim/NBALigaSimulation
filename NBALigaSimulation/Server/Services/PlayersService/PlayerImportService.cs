using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Teams;
using NBALigaSimulation.Shared.Models.Utils;
using Player = NBALigaSimulation.Shared.Models.Players.Player;
using Born = NBALigaSimulation.Shared.Models.Players.Born;

namespace NBALigaSimulation.Server.Services.PlayersService
{
    public class PlayerImportService : IPlayerImportService
    {
        private readonly IGenericRepository<Player> _playerRepository;
        private readonly IGenericRepository<Team> _teamRepository;
        private readonly IMapper _mapper;

        public PlayerImportService(
            IGenericRepository<Player> playerRepository,
            IGenericRepository<Team> teamRepository,
            IMapper mapper)
        {
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<PlayerCompleteDto>> CreatePlayer(CreatePlayerDto request)
        {
            ServiceResponse<PlayerCompleteDto> response = new ServiceResponse<PlayerCompleteDto>();

            Player player = _mapper.Map<Player>(request);

            await _playerRepository.AddAsync(player);
            await _playerRepository.SaveChangesAsync();

            var pos = string.IsNullOrWhiteSpace(player.Pos) ? "SF" : player.Pos;
            foreach (var rating in player.Ratings ?? new List<PlayerRatings>())
            {
                rating.ScoutReport = NBALigaSimulation.Shared.Engine.Scouting.ScoutingReportGenerator.Generate(rating, pos, player.Born?.Year);
            }
            await _playerRepository.SaveChangesAsync();

            response.Success = true;
            response.Data = _mapper.Map<PlayerCompleteDto>(player);
            return response;
        }

        public async Task<ServiceResponse<bool>> CreatePlayers(List<CreatePlayerDto> playersDto)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            if (playersDto == null || !playersDto.Any())
            {
                response.Success = false;
                response.Message = "Nenhum jogador foi fornecido para criação.";
                return response;
            }

            try
            {
                List<Player> players = new List<Player>();

                foreach (var playerDto in playersDto)
                {
                    if (await _teamRepository.Query().AnyAsync(t => t.Id == playerDto.TeamId))
                    {
                        Player player = _mapper.Map<Player>(playerDto);
                        players.Add(player);
                    }
                }

                await _playerRepository.AddRangeAsync(players);
                await _playerRepository.SaveChangesAsync();

                foreach (var player in players)
                {
                    var pos = string.IsNullOrWhiteSpace(player.Pos) ? "SF" : player.Pos;
                    foreach (var rating in player.Ratings)
                    {
                        rating.ScoutReport = NBALigaSimulation.Shared.Engine.Scouting.ScoutingReportGenerator.Generate(rating, pos, player.Born?.Year);
                    }
                }
                await _playerRepository.SaveChangesAsync();

                response.Data = true;
                response.Success = true;
                response.Message = $"Jogadores criados com sucesso! Total: {players.Count} jogador(es).";
            }
            catch (Exception ex)
            {
                response.Data = false;
                response.Success = false;
                response.Message = $"Erro ao criar jogadores: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> ImportBBGMPlayers(BBGMImportDto bbgmData)
        {
            var response = new ServiceResponse<bool>();

            if (bbgmData?.Players == null || !bbgmData.Players.Any())
            {
                response.Success = false;
                response.Message = "Nenhum jogador encontrado no JSON do BBGM.";
                return response;
            }

            try
            {
                var createPlayerDtos = new List<CreatePlayerDto>();

                foreach (var bbgmPlayer in bbgmData.Players)
                {
                    int teamId = bbgmPlayer.Tid == -2 ? 22 : bbgmPlayer.Tid;

                    var createDto = new CreatePlayerDto
                    {
                        Id = 0,
                        Name = $"{bbgmPlayer.FirstName} {bbgmPlayer.LastName}".Trim(),
                        Pos = bbgmPlayer.Pos ?? "",
                        College = bbgmPlayer.College ?? "",
                        Hgt = bbgmPlayer.Hgt,
                        Weight = bbgmPlayer.Weight,
                        ImgUrl = bbgmPlayer.ImgURL ?? "",
                        TeamId = teamId,
                        Born = bbgmPlayer.Born != null ? new Born
                        {
                            Year = bbgmPlayer.Born.Year,
                            Loc = bbgmPlayer.Born.Loc ?? ""
                        } : new Born { Year = 1990, Loc = "" },
                        Ratings = bbgmPlayer.Ratings?.Select(r => new PlayerRatingDto
                        {
                            Hgt = r.Hgt,
                            Stre = r.Stre,
                            Spd = r.Spd,
                            Jmp = r.Jmp,
                            Endu = r.Endu,
                            Ins = r.Ins,
                            Dnk = r.Dnk,
                            Ft = r.Ft,
                            Fg = r.Fg,
                            Tp = r.Tp,
                            Diq = r.Diq,
                            Oiq = r.Oiq,
                            Drb = r.Drb,
                            Pss = r.Pss,
                            Reb = r.Reb,
                            Pot = r.Pot
                        }).ToList() ?? new List<PlayerRatingDto>()
                    };

                    createPlayerDtos.Add(createDto);
                }

                var createResponse = await CreatePlayers(createPlayerDtos);
                
                response.Success = createResponse.Success;
                response.Message = createResponse.Success 
                    ? $"Importados {createPlayerDtos.Count} jogador(es) do BBGM. {createResponse.Message}"
                    : createResponse.Message;
                response.Data = createResponse.Success;

                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro ao importar jogadores do BBGM: {ex.Message}";
                return response;
            }
        }
    }
}
