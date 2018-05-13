using System.ComponentModel.DataAnnotations;

namespace DBO.Data.ViewModels
{
    public class EmployeeViewModel
    {
        public string Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "The name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "The email address has wrong format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Choose employee's image")]
        public string FilePath { get; set; }
    }
}
