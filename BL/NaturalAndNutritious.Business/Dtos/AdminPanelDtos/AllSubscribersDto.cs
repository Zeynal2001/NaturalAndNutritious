namespace NaturalAndNutritious.Business.Dtos.AdminPanelDtos
{
    public class AllSubscribersDto
    {
        public Guid Id { get; set; }
        public string SubscriberEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
