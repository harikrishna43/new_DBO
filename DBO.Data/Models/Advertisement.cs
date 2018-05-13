using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBO.Data.Models
{
    public enum AdvertisementStatus
    {
        Active,
        Paused,
        Stopped
    }

    public enum AdvertisementType
    {
        Image,
        Video
    }

    public enum LocationType
    {
        WithoutLocation,
        Country,
        Cities
    }

    public class Advertisement
    {
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        [MaxLength(50)]
        public string Headline { get; set; }

        [MaxLength(200)]
        public string Text { get; set; }

        public string Link { get; set; }
        [Column("FilePath")]
        public string FilePath { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int ClicksCount { get; set; }

        // Settings
        public string Location { get; set; }
        public bool IsFullWidth { get; set; }

        public decimal ClickPrice { get; set; }
        public decimal Budget { get; set; }
        public decimal BudgetSpent { get; set; }
        public AdvertisementStatus Status { get; set; }
        public AdvertisementType Type { get; set; }
        public LocationType LocationType { get; set; }



        // Settings flags
        public bool ApearOnLogin { get; set; }
        public bool ApearOnLogout { get; set; }
        public bool ApearForPrivatePerson { get; set; }
        public bool ApearForCompany { get; set; }

        public bool CreatedByAdmin { get; set; }

        public virtual ICollection<AdsIndustries> AdsIndustries { get; set; }
        public virtual ICollection<AdsSkills> AdsSkills { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
