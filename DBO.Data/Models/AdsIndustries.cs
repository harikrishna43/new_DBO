using System.ComponentModel.DataAnnotations.Schema;

namespace DBO.Data.Models
{
    public class AdsIndustries
    {
        public int Id { get; set; }
        public int IndustryId { get; set; }
        public Industry Industry { get; set; }

        public int AdvertisemenId { get; set; }
        [ForeignKey(nameof(AdvertisemenId))]
        public Advertisement Advertisement { get; set; }
    }
}
