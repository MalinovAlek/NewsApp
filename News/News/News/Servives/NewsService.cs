using News.Application.Models;
using Newtonsoft.Json;

namespace News.Application.Servives
{
    public class NewsService : INewsService
    {
        private const string BaseUrl = "https://hacker-news.firebaseio.com/v0";

        private readonly HttpClient _httpClient;

        private readonly ILogger<NewsService> _logger;

        public NewsService(IHttpClientFactory httpClientFactory, ILogger<NewsService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }
        public async Task<List<NewsModel>> GetNews()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{BaseUrl}/newstories.json");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newsIds = JsonConvert.DeserializeObject<List<int>>(responseBody);

                if (newsIds == null)
                {
                    return new List<NewsModel>();
                }

                //if (newsIds == null)
                //{
                //    throw new Exception("Failed to retrieve news IDs. Please try again later.");
                //}

                List<Task<NewsModel>> newsTasks = new List<Task<NewsModel>>();

                foreach (int newsId in newsIds)
                {
                    newsTasks.Add(FetchNewsItem(newsId));
                }

                var newsItems = await Task.WhenAll(newsTasks);

                return newsItems.ToList();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching news.");
                throw new Exception("Failed to retrieve news. Please try again later.", ex);
            }
        }

        public async Task<List<NewsModel>> GetFilteredNews(string filter)
        {
            List<NewsModel> newsItems = await GetNews();

            switch (filter.ToLower())
            {
                case "all":
                    newsItems = newsItems.OrderBy(news => news.Time).ToList();
                    break;
                case "hot":
                    newsItems = newsItems.OrderByDescending(news => news.Score).ToList();
                    break;
                case "show hn":
                    newsItems = newsItems.Where(news => news.Title.StartsWith("Show HN:", StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case "ask hn":
                    newsItems = newsItems.Where(news => news.Title.StartsWith("Ask HN:", StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                default:
                    break;
            }

            return newsItems;
        }
        public async Task<List<NewsModel>> SearchNews(string query)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"https://hn.algolia.com/api/v1/search?query={query}");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var searchResponse = JsonConvert.DeserializeObject<SearchResponseModel>(responseBody);

                if (searchResponse == null)
                {
                    return new List<NewsModel>();
                }

                //if (searchResponse == null)
                //{
                //    throw new Exception("Failed to retrieve search results. Please try again later.");
                //}

                List<NewsModel> searchResults = searchResponse.Hits.Select(hit => new NewsModel
                {
                    Id = hit.Id,
                    Title = hit.Title,
                    Author = hit.Author,
                    Score = hit.Points,
                    Type = hit.Type,
                    Url = hit.Url,
                    Time = hit.CreatedAt.Ticks
                }).ToList();

                return searchResults;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "An error occurred while searching for news.");
                throw new Exception("Failed to search for news. Please try again later.", ex);
            }
        }
        public async Task<NewsModel> GetNewsDetails(int newsId)
        {
            try
            {
                NewsModel newsItem = await FetchNewsItem(newsId);
                return newsItem;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "An error occurred while getting news details.");
                throw new Exception("Failed to retrieve news details. Please try again later.", ex);
            }
        }
        private async Task<NewsModel> FetchNewsItem(int newsId)
        {
            HttpResponseMessage newsResponse = await _httpClient.GetAsync($"{BaseUrl}/item/{newsId}.json");
            newsResponse.EnsureSuccessStatusCode();

            string newsBody = await newsResponse.Content.ReadAsStringAsync();
            var newsItem = JsonConvert.DeserializeObject<NewsModel>(newsBody);

            HttpResponseMessage commentResponse = await _httpClient.GetAsync($"{BaseUrl}/item/{newsId}.json");
            commentResponse.EnsureSuccessStatusCode();

            string commentBody = await commentResponse.Content.ReadAsStringAsync();
            var commentItem = JsonConvert.DeserializeObject<CommentModel>(commentBody);

            newsItem.CommentIds = commentItem.Kids;

            return newsItem;
        }
    }
}
