﻿using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Services.Results
{
    public class ServiceResult
    {
        public bool IsNull { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> CustomErrrors { get; set; }


        public ServiceResult()
        {
            IsNull = false;
            Succeeded = true;
            Message = string.Empty;
            CustomErrrors = new List<string>();
        }
    }
}
