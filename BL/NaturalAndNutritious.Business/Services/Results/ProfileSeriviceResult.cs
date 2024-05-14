using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Services.Results
{
    public class ProfileSeriviceResult
    {
        public bool IsNull { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> CustomErrrors { get; set; }
        //public Claim LoggedInUserId { get; set; }
        public AppUser FoundUser { get; set; }
        public ProfileIndexDto ProfileIndex { get; set; }
        public ProfileEditDto ProfileEdit { get; set; }


        public ProfileSeriviceResult()
        {
            IsNull = false;
            Succeeded = true;
            Message = "";//string.Empty;
            CustomErrrors = new List<string>();
            //LoggedInUserId = new Claim("NameIdentifier", "723424ASDASDDDaasdasdas12331");
            FoundUser = new AppUser();
            ProfileIndex = new ProfileIndexDto();
            ProfileEdit = new ProfileEditDto();
        }
    }

    public class SettedModels
    {
        public bool IsNull { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public ProfileIndexDto ProfileIndexModel { get; set; }
        public ProfileEditDto ProfileEditModel { get; set; }


        public SettedModels()
        {
            IsNull = false;
            Succeeded = true;
            Message = "";//string.Empty;
            ProfileIndexModel = new ProfileIndexDto();
            ProfileEditModel = new ProfileEditDto();
        }
    }

    public class EditResult
    {
        public bool IsNull { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }

        public EditResult()
        {
            IsNull = false; 
            Succeeded = true;
            Message = "";
        }
    }

    public class ChangePasswordResult
    {
        public bool IsNull { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }

        public ChangePasswordResult()
        {
            IsNull = false;
            Succeeded = true;
            Message = "";
        }
    }
}
