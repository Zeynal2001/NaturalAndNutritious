namespace NaturalAndNutritious.Business.Dtos
{
    public class CheckoutModel
    {
        public Guid ProductId { get; set; } //Bu ordere veya orderdetails a gedecek
        public int Quantity{ get; set; } //Bu orderdetails a gedecek
        public double Price{ get; set; } //Bu orderdetails a gedecek
        public double TotalPrice{ get; set; } //Bu ordere e gedecek
        public string Photo{ get; set; }
        public string Name{ get; set; }
    }
}
