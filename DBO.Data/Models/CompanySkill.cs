using System.ComponentModel.DataAnnotations.Schema;

namespace DBO.Data.Models
{
    public class CompanySkill
    {
        public int Id { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        [ForeignKey("Skill")]
        public int SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}
