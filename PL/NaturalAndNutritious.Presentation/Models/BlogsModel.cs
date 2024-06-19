namespace NaturalAndNutritious.Presentation.Models
{
    public class BlogsModel
    {
        public Guid Id { get; set; }
        public string? BlogPhotoUrl { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
