using DBO.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DBO.Data.Repositories
{
    public class SkillRepository
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        public IEnumerable<Skill> GetAll()
        {
            return _context.Skills.Include("CompanySkills");
        }

        public Skill Get(int skillId)
        {
            return _context.Skills.FirstOrDefault(x => x.Id == skillId);
        }

        public void Add(Skill skill)
        {
            _context.Skills.Add(skill);
            _context.SaveChanges();
        }
    }
}
