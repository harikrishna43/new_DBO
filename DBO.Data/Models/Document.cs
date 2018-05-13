using System.ComponentModel.DataAnnotations.Schema;

namespace DBO.Data.Models
{
    public class Document
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public int LanguageId { get; set; }

        [ForeignKey(nameof(LanguageId))]
        public Language Language { get; set; }
    }
}
