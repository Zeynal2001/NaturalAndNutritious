using NaturalAndNutritious.Business.Dtos;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Models
{
    public class AdminHomeVm
    {
        public double TotalBudget { get; set; }
        public int TotalViews { get; set; }
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
        public EmailModel? EmailModel { get; set; }
    }
}
