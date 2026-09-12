namespace PetShop.DataContext.Entites
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
        public bool IsPublished { get; set; } = true;
    }
}
