using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBO.Data.Models
{
    public class ClaimRequest
    {
        public int Id { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        [MaxLength(63)]
        public string Email { get; set; }

        public DateTime RequestTime { get; set; }

        public DateTime? ApproveTime { get; set; }

        public ClaimStatus ClaimStatus { get; set; }
    }

    public enum ClaimStatus
    {
        Created,
        Approved,
        Rejected
    }
}
