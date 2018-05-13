using System.ComponentModel.DataAnnotations;

namespace DBO.Data.ViewModels
{
    public class ResourceViewModel
    {
        public int? Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Value { get; set; }

        public int LanguageId { get; set; }
    }
}
