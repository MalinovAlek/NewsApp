namespace News.Application.Models
{
    public class NewsModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string? Author { get; set; }
        public int Score { get; set; }
        public string? Type { get; set; }
        public string? Url { get; set; }
        public long Time { get; set; }
        public List<int>? CommentIds { get; set; }
    }
}