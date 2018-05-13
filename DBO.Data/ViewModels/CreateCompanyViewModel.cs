using DBO.Data.ValidationAttributes;

using System.ComponentModel.DataAnnotations;

namespace DBO.Data.ViewModels
{
    public class CreateCompanyViewModel
    {
        [LocalizedDisplayName("CompanyName")]
        [Required]
        public string Name { get; set; }

        [LocalizedDisplayName("Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [LocalizedDisplayName("Phone")]
        [Required]
        public string Phone { get; set; }

        public string Role { get; set; }
    }
}
