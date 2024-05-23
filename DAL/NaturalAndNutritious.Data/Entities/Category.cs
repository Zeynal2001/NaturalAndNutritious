namespace NaturalAndNutritious.Data.Entities
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<SubCategory> SubCategories { get; set; }
    }
}
