namespace NaturalAndNutritious.Presentation.Models
{
    public class OrdersModel
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderNo { get; set; }
        public string OrderStatus { get; set; }
        public double TotalAmount { get; set; }
        public int ProductQuantity { get; set; }
    }
}
