namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Models
{
    public class SeeMessageModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public DateTime MessageDate { get; set; }
        public string MessageText { get; set; }
    }
}
