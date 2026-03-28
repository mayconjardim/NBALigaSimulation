using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Server.Services.DraftService;
using NBALigaSimulation.Server.Services.PlayersService;
using NBALigaSimulation.Shared.Dtos.Seasons;
using NBALigaSimulation.Shared.Engine.Schedule;
using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.SeasonService
{
    public class SeasonService : ISeasonService
    {
        private readonly ISeasonRepository _seasonRepository;
        private readonly IDraftService _draftService;
        private readonly IPlayerService _playerService;
        private readonly IFAService _faService;
        private readonly IMapper _mapper;

        public SeasonService(
            ISeasonRepository seasonRepository,
            IDraftService draftService,
            IPlayerService playerService,
            IFAService faService,
            IMapper mapper)
        {
            _seasonRepository = seasonRepository;
            _draftService = draftService;
            _playerService = playerService;
            _faService = faService;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<CompleteSeasonDto>> GetLastSeason()
        {
            var response = new ServiceResponse<CompleteSeasonDto>();
            var season = await _seasonRepository.GetLastSeasonAsync();

            if (season == null)
            {
                response.Success = false;
                response.Message = $"Temporada não econtrada!";
            }
            else
            {
                response.Data = _mapper.Map<CompleteSeasonDto>(season);
            }

            return response;
        }
        
        public async Task<ServiceResponse<List<CompleteSeasonDto>>> GetALlSeason()
        {
            ServiceResponse<List<CompleteSeasonDto>> response = new ServiceResponse<List<CompleteSeasonDto>>();
            
            var seasons = await _seasonRepository.GetAllOrderedAsync();
            
            if (seasons.Count == 0)
            {
                response.Success = false;
                response.Message = $"Não existem temporadas!";
            }
            else
            {
                response.Success = true;
                response.Data = _mapper.Map<List<CompleteSeasonDto>>(seasons);
                response.Message = $"Temporadas retornadas com sucesso!";

            }
            
            return response;
        }

        public async Task<ServiceResponse<CompleteSeasonDto>> CreateSeason()
        {
            ServiceResponse<CompleteSeasonDto> response = new ServiceResponse<CompleteSeasonDto>();

            var LastSeason = await _seasonRepository.GetLastSeasonAsync();

            if (LastSeason == null)
            {
                response.Success = false;
                response.Message = $"Temporada não econtrada!";
                return response;
            }

            if (!LastSeason.IsCompleted)
            {
                response.Success = false;
                response.Message = $"Temporada ainda não finalizada";
                return response;
            }

            Season season = new Season
            {
                Year = LastSeason.Year + 1
            };

            await _seasonRepository.AddAsync(season);
            await _seasonRepository.SaveChangesAsync();

            await _faService.DeleteOffersBySeason(LastSeason.Year);

            await _playerService.HandleOffSeasonRecoveryAsync(30);

            await _playerService.HandleExpiredContractsAsync(season.Year);

            await _draftService.GenerateFuturePicks(season.Year);

            response.Message = $"Temporada criada com sucesso!";
            response.Success = true;
            response.Data = _mapper.Map<CompleteSeasonDto>(season);
            return response;
        }

        public async Task<ServiceResponse<CompleteSeasonDto>> GenerateSchedule()
        {
            ServiceResponse<CompleteSeasonDto> response = new ServiceResponse<CompleteSeasonDto>();

            Season season = await _seasonRepository.GetLastSeasonAsync();

            if (season == null)
            {
                response.Success = false;
                response.Message = "Temporada não encontrada.";
                return response;
            }

            List<Game> existingGames = await _seasonRepository.GetGamesBySeasonAsync(season.Id);

            if (existingGames.Count > 0)
            {
                response.Success = false;
                response.Message = "Já existem jogos para esta temporada.";
                return response;
            }

            List<Team> teams = await _seasonRepository.GetHumanTeamsAsync();

            if (teams.Count == 0)
            {
                response.Success = false;
                response.Message = "Não há equipes disponíveis para criar o cronograma.";
                return response;
            }

            List<Game> generatingSeasonGames = ScheduleHelp.GenerateSchedule(teams, season);

            if (generatingSeasonGames.Count == 0)
            {
                response.Success = false;
                response.Message = "Não foram gerador jogos para a temporada.";
                return response;
            }
            
            await _seasonRepository.AddGamesAsync(generatingSeasonGames);
            await _seasonRepository.SaveChangesAsync();

            List<Game> savedGames = await _seasonRepository.GetGamesWithTeamsBySeasonAsync(season.Id);
            
            // Distribui os jogos em rodadas (3 jogos por rodada por time por padrão)
            var games = ScheduleHelp.GenerateRounds(savedGames, teams, gamesPerRoundPerTeam: 3);
            await _seasonRepository.SaveChangesAsync();
            
            response.Message = "Cronograma criado com sucesso!";
            response.Success = true;
            response.Data = _mapper.Map<CompleteSeasonDto>(season);

            return response;
        }

        public async Task<ServiceResponse<CompleteSeasonDto>> GenerateTrainingCamp()
        {
            ServiceResponse<CompleteSeasonDto> response = new ServiceResponse<CompleteSeasonDto>();

            Season season = await _seasonRepository.GetLastSeasonAsync();

            if (season == null)
            {
                response.Success = false;
                response.Message = "Temporada não encontrada.";
                return response;
            }

            var tcResponse = await _playerService.GenerateTrainingCampAsync(season);

            if (!tcResponse.Success)
            {
                response.Success = false;
                response.Message = tcResponse.Message;
                return response;
            }

            response.Success = true;
            response.Message = tcResponse.Message;
            response.Data = _mapper.Map<CompleteSeasonDto>(season);

            return response;
        }
        
        public async Task<ServiceResponse<CompleteSeasonDto>> CleanSchedule()
        {
            
            ServiceResponse<CompleteSeasonDto> response = new ServiceResponse<CompleteSeasonDto>();
            
            Season season = await _seasonRepository.GetLastSeasonAsync();
            
            List<Game> existingGames = await _seasonRepository.GetGamesBySeasonAsync(season.Id);

            if (existingGames.Count <= 0)
            {
                response.Success = false;
                response.Message = "Não existem jogos!";
                return response;
            }

            await _seasonRepository.RemoveGamesAsync(existingGames);
            await _seasonRepository.SaveChangesAsync();
            
            response.Success = true;
            response.Message = "Jogos deletados com sucesso!";
            return response;
          
        }


    }
}
