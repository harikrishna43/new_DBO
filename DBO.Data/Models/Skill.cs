using DBO.Data.ValidationAttributes;

using System.Collections.Generic;

namespace DBO.Data.Models
{
    public class Skill : INamedEntity
    {
        public int Id { get; set; }

        [LocalizedDisplayName("Name")]
        public string Name { get; set; }

        public virtual ICollection<CompanySkill> CompanySkills { get; set; }
    }
}
