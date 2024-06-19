namespace NaturalAndNutritious.Business.Dtos.AdminPanelDtos
{
    public class AllOrdersDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string RecipientName { get; set; }
        public string MobileNumber { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public double Freight { get; set; }
        public string ShipName { get; set; } //Nəqliyyatçı şirkəti adı
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegion { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipCountry { get; set; }
        public bool Confirmed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}