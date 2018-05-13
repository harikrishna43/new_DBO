using System;

namespace DBO.Data.ViewModels
{
    public class BaseCompanyInfoViewModel
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Cvr { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public Guid? Token { get; set; }
    }
}
