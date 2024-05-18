using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Services.Results
{
    public class AdminServicesResult
    {
        public bool IsNull { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public ClaimsPrincipal UserClaimsPrincipal { get; set; }

        public AdminServicesResult()
        {
            IsNull = false;
            Succeeded = true;
            Message = "";//string.Empty;
            UserClaimsPrincipal = new ClaimsPrincipal();
        }
    }
}
