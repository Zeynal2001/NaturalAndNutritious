namespace NaturalAndNutritious.Data.Entities
{
    public class Blog : BaseEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string? BlogPhotoUrl { get; set; }
        public string? AdditionalPhotoUrl1 { get; set; }
        public string? AdditionalPhotoUrl2 { get; set; }
        public int ViewsCount { get; set; }
    }
}