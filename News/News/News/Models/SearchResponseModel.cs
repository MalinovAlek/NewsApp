namespace News.Application.Models
{
    public class SearchResponseModel
    {
        public List<Hit> Hits { get; set; }
    }

    public class Hit
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string? Author { get; set; }
        public int Points { get; set; }
        public string? Type { get; set; }
        public string? Url { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
