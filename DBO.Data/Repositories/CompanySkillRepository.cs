using DBO.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace DBO.Data.Repositories
{
    public class CompanySkillRepository
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        public Company Get(int id)
        {
            return _context.Companies.Find(id);
        }

        public IEnumerable<CompanySkill> GetSkills(int companyId)
        {
            return _context.CompanySkills.Include(nameof(CompanySkill.Skill))
                                         .Where(x => x.CompanyId == companyId)
                                         .ToList();
        }

        public void Add(CompanySkill companySkill)
        {
            _context.CompanySkills.Add(companySkill);
            _context.SaveChanges();
        }

        public void Remove(int companyId, int skillId)
        {
            var companySkill = _context.CompanySkills.FirstOrDefault(x => x.CompanyId == companyId && x.SkillId == skillId);
            _context.CompanySkills.Remove(companySkill);
            _context.SaveChanges();
        }
    }
}
