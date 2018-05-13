using DBO.Data;
using DBO.Data.Models;
using DBO.Services.Contract;
using System.Collections.Generic;
using System.Linq;

namespace DBO.Services.Implementation
{
    public class SubdataService : ISubdataService
    {
        private readonly ApplicationDbContext _context;

        public SubdataService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<INamedEntity> GetSkills()
        {
            return _context.Skills.ToList();
        }

        public IEnumerable<INamedEntity> GetIndustries()
        {
            return _context.Industries.ToList();
        }
    }
}
