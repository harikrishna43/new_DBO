using DBO.Data.ValidationAttributes;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBO.Data.Models
{
    public class Company
    {
        public int Id { get; set; }
        public int CVR { get; set; }

        [Index]
        [MaxLength(255)]
        [LocalizedDisplayName("Name")]
        public string Name { get; set; }

        [MaxLength(255)]
        [LocalizedDisplayName("Address")]
        public string Address { get; set; }

        [MaxLength(15)]
        [LocalizedDisplayName("PostCode")]
        public string PostCode { get; set; }

        [Index]
        [MaxLength(255)]
        [LocalizedDisplayName("City")]
        public string City { get; set; }

        [Index]
        [MaxLength(15)]
        [LocalizedDisplayName("Phone")]
        public string Phone { get; set; }

        [MaxLength(255)]
        [LocalizedDisplayName("Web")]
        public string Web { get; set; }

        [Index]
        [MaxLength(255)]
        [LocalizedDisplayName("Email")]
        public string Email { get; set; }

        [MaxLength(255)]
        [LocalizedDisplayName("PersonName")]
        public string PersonName { get; set; }

        [MaxLength(255)]
        [LocalizedDisplayName("Chairman")]
        public string Chairman { get; set; }

        [LocalizedDisplayName("IndustryCode")]
        public int? IndustryCode { get; set; }

        [MaxLength(255)]
        [LocalizedDisplayName("IndustryText")]
        public string IndustryText { get; set; }

        [LocalizedDisplayName("AdvertisingProtection")]
        public bool AdvertisingProtection { get; set; }

        [MaxLength(255)]
        [LocalizedDisplayName("Owner")]
        public string Owner { get; set; }

        [MaxLength(255)]
        [LocalizedDisplayName("Image")]
        public string Image { get; set; }

        [MaxLength(255)]
        [LocalizedDisplayName("TextDescription")]
        public string TextDescription { get; set; }

        [LocalizedDisplayName("Connections")]
        public int Connections { get; set; }

        [ForeignKey("Industry")]
        public int? IndustryId { get; set; }
        [LocalizedDisplayName("Industry")]
        public Industry Industry { get; set; }

        [MaxLength(127)]
        [LocalizedDisplayName("Title")]
        public string Title { get; set; }

        [LocalizedDisplayName("IsProcessed")]
        public bool IsProcessed { get; set; }

        [MaxLength(127)]
        [LocalizedDisplayName("Country")]
        public string Country { get; set; }

        public virtual ICollection<CompanySkill> CompanySkills { get; set; }
        public virtual ICollection<ApplicationUser> Employees { get; set; }
        public virtual ICollection<ClaimRequest> ClaimRequests { get; set; }
        public virtual ICollection<RegistrationCode> RegistrationCodes { get; set; }
        public virtual ICollection<ScheduledEmail> ScheduledEmails { get; set; }
    }
}