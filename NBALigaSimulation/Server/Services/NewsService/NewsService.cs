using AutoMapper;
using NBALigaSimulation.Shared.Dtos.GameNews;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.NewsService
{
    public class NewsService : INewsService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public NewsService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<NewsDto>>> GetAllNews()
        {
            var news = await _context.GameNews.OrderByDescending(n => n.Id)
                                               .Take(23)
                                               .ToListAsync();

            var response = new ServiceResponse<List<NewsDto>>
            {
                Data = _mapper.Map<List<NewsDto>>(news)
            };

            return response;
        }

    }
}
