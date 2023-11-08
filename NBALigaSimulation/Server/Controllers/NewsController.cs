using Microsoft.AspNetCore.Mvc;
using NBALigaSimulation.Server.Services.NewsService;

namespace NBALigaSimulation.Server.Controllers
{
    [Route("api/news")]
    [ApiController]
    public class NewsController : ControllerBase
    {

        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<NewsDto>>>> GetAllNews()
        {

            var result = await _newsService.GetAllNews();
            return Ok(result);

        }

    }
}
