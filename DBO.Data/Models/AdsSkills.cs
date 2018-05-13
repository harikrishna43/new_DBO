using System.ComponentModel.DataAnnotations.Schema;

namespace DBO.Data.Models
{
    public class AdsSkills
    {
        public int Id { get; set; }
        public int SkillId { get; set; }
        public Skill Skill { get; set; }

        public int AdvertisementId { get; set; }
        [ForeignKey(nameof(AdvertisementId))]
        public Advertisement Advertisement { get; set; }
    }
}
