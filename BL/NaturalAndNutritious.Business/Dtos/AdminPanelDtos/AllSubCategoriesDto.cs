namespace NaturalAndNutritious.Business.Dtos.AdminPanelDtos
{
    public class AllSubCategoriesDto
    {
        public string Id { get; set; }
        public string SubCategoryName { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
