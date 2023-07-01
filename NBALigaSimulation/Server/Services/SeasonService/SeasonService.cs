using AutoMapper;
using Microsoft.EntityFrameworkCore;
using static MudBlazor.CategoryTypes;

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

        public async Task<ServiceResponse<CompleteSeasonDto>> CreateSeason(CreateSeasonDto request)
        {
            ServiceResponse<CompleteSeasonDto> response = new ServiceResponse<CompleteSeasonDto>();

            Season season = _mapper.Map<Season>(request);

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
                        Year = season.Year,
                        Round = round
                    };

                    draftPicks.Add(draftPick);
                }

                _context.AddRange(draftPicks);
            }

            await _context.SaveChangesAsync();

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
                response.Message = $"Season não econtrada!";
            }
            else
            {
                response.Data = _mapper.Map<CompleteSeasonDto>(season);
            }

            return response;
        }


        public async Task<ServiceResponse<CompleteSeasonDto>> GenerateSchedule(int seasonId)
        {
            ServiceResponse<CompleteSeasonDto> response = new ServiceResponse<CompleteSeasonDto>();

            Season season = await _context.Seasons
                .Include(s => s.Games)
                .FirstOrDefaultAsync(p => p.Id == seasonId);

            if (season == null)
            {
                response.Success = false;
                response.Message = "Season not found.";
                return response;
            }

            List<Team> teams = await _context.Teams.Where(t => t.IsHuman == true).ToListAsync();

            season.NewSchedule(teams);

            await _context.SaveChangesAsync();

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

            foreach (Player player in players)
            {
                var newRatings = player.TrainingCamp(season);

                player.Ratings.Add(newRatings);
            }

            await _context.SaveChangesAsync();

            response.Success = true;
            response.Data = _mapper.Map<CompleteSeasonDto>(season);

            return response;
        }



    }
}
