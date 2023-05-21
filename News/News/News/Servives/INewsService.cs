using News.Application.Models;

namespace News.Application.Servives
{
    public interface INewsService
    {
        Task<List<NewsModel>> GetNews();
        Task<List<NewsModel>> GetFilteredNews(string filter);
        Task<List<NewsModel>> SearchNews(string query);
        Task<NewsModel> GetNewsDetails(int newsId);
    }
}
