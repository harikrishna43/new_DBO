using DBO.Data.Models;
using DBO.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DBO.Data.Repositories
{
    public class TranslationRepository
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        public async Task<IDictionary<string, List<Resource>>> GetAllGroupedByName()
        {
            var whitelistedNames = new string[]{ "English", "Danish" };
            return await _db.Resources
                            .Where(x => whitelistedNames.Contains(x.Language.Name))
                            .GroupBy(x => x.Name)
                            .ToDictionaryAsync(g => g.Key, g => g.ToList());
        }

        public async Task<Resource> Get(int id)
        {
            return await _db.Resources.FindAsync(id);
        }

        public void Delete(int id)
        {

        }

        public async Task Update(int? id, string name, string value, int languageId)
        {
            if (id == null)
            {
                var resource = new Resource
                {
                    LanguageId = languageId,
                    Name = name,
                    Value = value
                };
                _db.Resources.Add(resource);
            }
            else
            {
                var res = _db.Resources.FirstOrDefault(r => r.Id == id.Value);

                if (res != null)
                {
                    res.Value = value;
                    _db.Entry(res).State = EntityState.Modified;
                }
            }

            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Resource>> GetByName(string name)
        {
            return await _db.Resources.Include(nameof(Language))
                                .Where(x => x.Name == name)
                                .ToListAsync();

        }
    }
}
