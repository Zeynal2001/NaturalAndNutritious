using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Dtos.AdminPanelDtos
{
    public class AllUsersDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string? ProfilePhoto { get; set; }
    }
}
