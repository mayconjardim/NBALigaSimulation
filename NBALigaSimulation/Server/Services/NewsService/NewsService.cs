using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Shared.Dtos.GameNews;
using NBALigaSimulation.Shared.Models.GameNews;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.NewsService
{
    public class NewsService : INewsService
    {

        private readonly IGenericRepository<News> _newsRepository;
        private readonly IMapper _mapper;

        public NewsService(
            IGenericRepository<News> newsRepository,
            IMapper mapper)
        {
            _newsRepository = newsRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<NewsDto>>> GetAllNews()
        {
            var news = await _newsRepository.Query()
                .OrderByDescending(n => n.Id)
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
