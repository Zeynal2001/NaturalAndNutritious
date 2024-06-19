namespace NaturalAndNutritious.Business.Dtos.AdminPanelDtos
{
    public class AllMessagesDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmailAddress { get; set; }
        //public string CustomerMessage { get; set; }
        public bool IsAnswered { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
