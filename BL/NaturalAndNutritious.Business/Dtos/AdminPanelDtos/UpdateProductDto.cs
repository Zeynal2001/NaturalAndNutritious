using Microsoft.AspNetCore.Http;
using NaturalAndNutritious.Business.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Dtos.AdminPanelDtos
{
    public class UpdateProductDto
    {
        public string Id { get; set;}
        [Required]
        public string ProductName { get; set;}
        [Required]
        public string ShortDescription { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ProductImageUrl { get; set; }
        [FileValidator(AcceptedTypes = ".png, .jpg, .jpeg, .svg")]
        public IFormFile? ProductImage { get; set; }
        [Required]
        public double ProductPrice { get; set; }
        [Required]
        public int UnitsInStock { get; set; }
        [Required]
        public int UnitsOnOrder { get; set; }
        [Required]
        public int ReorderLevel { get; set; }

        //public string Supplier { get; set; }
        //public string Category { get; set; }
        //public string SubCategory { get; set; }
    }
}
