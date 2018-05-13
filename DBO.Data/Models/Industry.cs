using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DBO.Data.Models
{
    public class Industry : INamedEntity
    {
        public int Id { get; set; }

        [MaxLength(127)]
        public string Name { get; set; }

        public virtual ICollection<Company> Companies { get; set; }
    }
}
