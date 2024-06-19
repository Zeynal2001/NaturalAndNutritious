namespace NaturalAndNutritious.Data.Entities
{
    public class ContactMessage : BaseEntity
    {
        //public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmailAddress { get; set; }
        public string CustomerMessage { get; set; }
        public bool IsAnswered { get; set; }
    }
}