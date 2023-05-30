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

    }
}
