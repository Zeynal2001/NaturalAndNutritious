﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Services.Results
{
    public class CategoryServiceResult
    {
        public bool IsNull { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }

        public CategoryServiceResult()
        {
            IsNull = false;
            Succeeded = true;
            Message = string.Empty;
        }
    }
}
