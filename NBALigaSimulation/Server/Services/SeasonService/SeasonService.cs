using AutoMapper;
using Microsoft.EntityFrameworkCore;

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


        public async Task<ServiceResponse<CompleteSeasonDto>> UpdateSeason(int seasonId)
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

            List<Team> teams = await _context.Teams.ToListAsync();

            season.NewSchedule(teams); // Gera os jogos

            await _context.SaveChangesAsync();

            response.Success = true;
            response.Data = _mapper.Map<CompleteSeasonDto>(season);

            return response;
        }

    }
}
