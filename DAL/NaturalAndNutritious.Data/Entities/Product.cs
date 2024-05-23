namespace NaturalAndNutritious.Data.Entities
{
    public class Product : BaseEntity
    {
        public string ProductName { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string? ProductImageUrl { get; set; }
        public double ProductPrice { get; set; }
        public int UnitsInStock { get; set; }
        public int UnitsOnOrder { get; set; }
        public int ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        public int ViewsCount { get; set; }

        public IEnumerable<Review> Reviews { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
        //TODO: Admin panelini quranda aşağıda olan nullable ları sil.
        public Guid? CategoryId { get; set; }
        public Guid? SubCategoryId { get; set; }
        public Guid? SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public Category Category { get; set; }
        public SubCategory SubCategory { get; set; }
        public Discount? Discount { get; set; }
    }
}
