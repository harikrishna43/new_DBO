using DBO.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace DBO.Data.ViewModels
{
    public class LanguageViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public LanguageViewModel()
        {

        }

        public LanguageViewModel(Language language)
        {
            Id = language.Id;
            Name = language.Name;
        }
    }
}
