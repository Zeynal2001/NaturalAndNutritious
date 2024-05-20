using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Dtos.AdminPanelDtos
{
    public class CreateSubcategoryDto
    {
        [Required]
        public string SubCategoryName { get; set; }
        [Required]
        public string Category { get; set; }
    }
}
