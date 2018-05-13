using DBO.Data.ValidationAttributes;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBO.Data.Models
{
    public class RegistrationCode
    {
        public Guid Id { get; set; }
        public DateTime? Generated { get; set; }
        public DateTime? Registered { get; set; }

        [ForeignKey("Company")]
        public int?  CompanyId { get; set; }
        public Company Company { get; set; }

        public int Cvr { get; set; }

        [MaxLength(255)]
        [LocalizedDisplayName("Name")]
        public string Name { get; set; }

        [MaxLength(255)]
        [LocalizedDisplayName("Email")]
        public string Email { get; set; }

        public bool IsCompany { get; set; }
    }
}
