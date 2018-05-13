using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBO.Data.Models
{
    public class AdClick
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        public int AdvertisementId { get; set; }
        [ForeignKey(nameof(AdvertisementId))]
        public Advertisement Advertisement { get; set; }

        [MaxLength(15)]
        public string IpAddress { get; set; }

        public DateTime DateTime { get; set; }

        /// <summary>
        /// Amount of money, charged for this click
        /// </summary>
        public decimal Charged { get; set; }
    }
}
