using System.ComponentModel.DataAnnotations;

namespace DBO.Models
{
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}