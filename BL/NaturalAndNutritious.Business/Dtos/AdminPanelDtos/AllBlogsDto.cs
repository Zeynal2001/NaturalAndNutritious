namespace NaturalAndNutritious.Business.Dtos.AdminPanelDtos
{
    public class AllBlogsDto
    {
        public Guid Id { get; set; }
        public string BlogPhoto { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
