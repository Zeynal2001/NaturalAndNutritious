namespace NaturalAndNutritious.Data.Entities
{
    public class Territory : BaseEntity
    {
        public string TerritoryDescription { get; set; }
        public Guid RegionId { get; set; }
        public Region Region { get; set; }
    }
}
