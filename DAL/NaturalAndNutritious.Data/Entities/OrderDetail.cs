namespace NaturalAndNutritious.Data.Entities
{
    public class OrderDetail : BaseEntity
    {
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }

        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
