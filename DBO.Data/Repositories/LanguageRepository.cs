using DBO.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DBO.Data.Repositories
{
    public class LanguageRepository
    {
        private readonly ApplicationDbContext _context;

        public LanguageRepository()
        {
            _context = new ApplicationDbContext();
        }

        public async Task<Language> GetByName(string name)
        {
            return await _context.Languages.FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<Language> Get(int id)
        {
            return await _context.Languages.FindAsync(id);
        }

        public async Task<IEnumerable<Language>> GetAll()
        {
            return await _context.Languages.ToListAsync();
        }

        public async Task Delete(int id)
        {
            var language = await _context.Languages.FindAsync(id);
            if (language != null)
            {
                _context.Languages.Remove(language);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Update(int id,string name)
        {
            var language = await _context.Languages.FindAsync(id);

            if (language != null)
            {
                language.Name = name;
                _context.Entry<Language>(language).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task Create(string name)
        {
            _context.Languages.Add(new Language { Name = name });
            await _context.SaveChangesAsync();
        }
    }
}
