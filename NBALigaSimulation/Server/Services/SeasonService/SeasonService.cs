using AutoMapper;

namespace NBALigaSimulation.Server.Services.SeasonService
{
    public class SeasonService : ISeasonService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public SeasonService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<CompleteSeasonDto>> CreateSeason()
        {
            ServiceResponse<CompleteSeasonDto> response = new ServiceResponse<CompleteSeasonDto>();

            var LastSeason = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();

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

            Season season = new Season();
            season.Year = LastSeason.Year + 1;

            _context.Add(season);
            await _context.SaveChangesAsync();

            List<Team> teams = await _context.Teams.Where(t => t.IsHuman == true).ToListAsync();

            foreach (Team team in teams)
            {
                List<TeamDraftPicks> draftPicks = new List<TeamDraftPicks>();

                for (int round = 1; round <= 2; round++)
                {
                    TeamDraftPicks draftPick = new TeamDraftPicks
                    {
                        TeamId = team.Id,
                        Original = team.Abrv,
                        Year = season.Year + 3,
                        Round = round
                    };

                    draftPicks.Add(draftPick);
                }

                _context.AddRange(draftPicks);
            }

            await _context.SaveChangesAsync();

            response.Message = $"Temporada criada com sucesso!";
            response.Success = true;
            response.Data = _mapper.Map<CompleteSeasonDto>(season);
            return response;
        }

        public async Task<ServiceResponse<CompleteSeasonDto>> GetLastSeason()
        {
            var response = new ServiceResponse<CompleteSeasonDto>();
            var season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();

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


        public async Task<ServiceResponse<CompleteSeasonDto>> GenerateSchedule()
        {
            ServiceResponse<CompleteSeasonDto> response = new ServiceResponse<CompleteSeasonDto>();

            Season season = await _context.Seasons.OrderBy(s => s.Year).LastOrDefaultAsync();

            if (season == null)
            {
                response.Success = false;
                response.Message = "Temporada não encontrada.";
                return response;
            }

            List<Game> games = await _context.Games.Where(s => s.SeasonId == season.Id).ToListAsync();

            if (games.Count > 0)
            {
                response.Success = false;
                response.Message = "Já existe um jogos pra essa temporada.";
                return response;
            }

            List<Team> teams = await _context.Teams.Where(t => t.IsHuman == true).ToListAsync();

            season.NewSchedule(teams);

            await _context.SaveChangesAsync();

            response.Message = "Schedule criado com sucesso!";
            response.Success = true;
            response.Data = _mapper.Map<CompleteSeasonDto>(season);

            return response;
        }

        public async Task<ServiceResponse<CompleteSeasonDto>> GenerateTrainingCamp()
        {
            ServiceResponse<CompleteSeasonDto> response = new ServiceResponse<CompleteSeasonDto>();

            Season season = await _context.Seasons.OrderBy(s => s.Id).LastOrDefaultAsync();

            if (season == null)
            {
                response.Success = false;
                response.Message = "Temporada não encontrada.";
                return response;
            }

            List<Player> players = await _context.Players.Include(p => p.Ratings).ToListAsync();

            if (players.Any(p => p.Ratings.Where(s => s.Season == season.Year).Any()))
            {
                response.Success = false;
                response.Message = "TrainingCamp já realizado!";
                return response;
            }

            foreach (Player player in players)
            {
                var newRatings = TcUtils.TrainingCamp(player, season);

                player.Ratings.Add(newRatings);
            }

            await _context.SaveChangesAsync();

            response.Success = true;
            response.Data = _mapper.Map<CompleteSeasonDto>(season);

            return response;
        }



    }
}
