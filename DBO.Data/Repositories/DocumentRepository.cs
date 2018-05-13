using DBO.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBO.Data.Repositories
{
    public class DocumentRepository
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        public async Task<IEnumerable<string>> GetAll()
        {
            return await _context.Documents.GroupBy(x => x.Name).Select(x => x.Key).ToListAsync();
        }

        public async Task<Document> Get(int id)
        {
            return await _context.Documents.FindAsync(id);
        }

        public async Task<IEnumerable<Document>> GetByName(string name)
        {
            return await _context.Documents.Where(x => x.Name == name).ToListAsync();
        }


        public async Task Update(int id, string content, string name, int languageId)
        {
            if (id == 0)
            {
                var document = new Document
                {
                    Name = name,
                    Content = content,
                    LanguageId = languageId
                };

                _context.Documents.Add(document);
            }
            else
            {
                var document = await Get(id);
                if (document != null)
                {
                    document.Content = content;
                    _context.Entry(document).State = EntityState.Modified;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
