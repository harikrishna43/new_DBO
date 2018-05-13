using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBO.Data.Models
{
    public class Resource
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public int LanguageId { get; set; }

        [ForeignKey(nameof(LanguageId))]
        public Language Language { get; set; }

        public Resource()
        {

        }

        public Resource(string name, string value, int languageId)
        {
            Name = name;
            Value = value;
            LanguageId = languageId;
        }
    }
}
