using DBO.Common;
using DBO.Data.Models;

namespace DBO.Data.ViewModels
{
    public class AdminCompanyViewModel
    {
        public Company Company { get; set; }
        public CompanyNavigationViewModel CompanyNavigation { get; set; }
        public SearchParams SearchParams { get; set; }
        public bool IsClaimed { get; set; }
    }
}
