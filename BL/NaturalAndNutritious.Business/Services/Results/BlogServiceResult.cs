using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Services.Results
{
    public class BlogServiceResult
    {
        public bool IsNull { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }

        public BlogServiceResult()
        {
            IsNull = false;
            Succeeded = true;
            Message = string.Empty;
        }
    }
}
