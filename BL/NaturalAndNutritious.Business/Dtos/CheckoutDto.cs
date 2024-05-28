namespace NaturalAndNutritious.Business.Dtos
{
    public class CheckoutDto
    {
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegion { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipCountry { get; set; }
        public bool CashOnDelivery { get; set; }
    }
}
