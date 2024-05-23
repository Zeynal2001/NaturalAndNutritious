namespace NaturalAndNutritious.Data.Entities
{
    public class Blog : BaseEntity
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string BlogPhotoUrl { get; set; }
        public int ViewsCount { get; set; }
    }
}
