namespace NaturalAndNutritious.Data.Entities
{
    public class SubCategory : BaseEntity
    {
        public string SubCategoryName { get; set; }
        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
