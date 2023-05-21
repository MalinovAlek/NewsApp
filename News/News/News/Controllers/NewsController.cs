using Microsoft.AspNetCore.Mvc;
using News.Application.Servives;
using News.Application.Models;

namespace News.Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewsApiController : ControllerBase
    {
        private readonly INewsService _newsService;
        private readonly ILogger<NewsApiController> _logger;

        public NewsApiController(INewsService newsService, ILogger<NewsApiController> logger)
        {
            _newsService = newsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetNews()
        {
            try
            {
                var newsItems = await _newsService.GetNews();
                return Ok(newsItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching news.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{newsId}")]
        public async Task<IActionResult> GetNewsDetails(int newsId)
        {
            try
            {
                var newsDetails = await _newsService.GetNewsDetails(newsId);
                if (newsDetails != null)
                {
                    return Ok(newsDetails);
                }
                else
                {
                    return NotFound($"News item with ID {newsId} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching news details.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredNews(string filter)
        {
            try
            {
                var filteredNewsItems = await _newsService.GetFilteredNews(filter);
                return Ok(filteredNewsItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching filtered news.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchNews(string query)
        {
            try
            {
                var searchResults = await _newsService.SearchNews(query);
                return Ok(searchResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for news.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}