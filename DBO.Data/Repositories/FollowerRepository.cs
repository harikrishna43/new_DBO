using DBO.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBO.Data.Repositories
{
    public class FollowerRepository
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        public Follower Get(Guid userId, int companyId)
        {
            return _context.Followers.FirstOrDefault(x => x.UserId == userId && x.CompanyId == companyId);
        }

        public async Task CreateAsync(int companyId, Guid userId)
        {
            var follower = new Follower
            {
                CompanyId = companyId,
                UserId = userId
            };            
            _context.Followers.Add(follower);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int companyId, Guid userId)
        {
            var follower = _context.Followers.FirstOrDefault(x => x.CompanyId == companyId && x.UserId == userId);

            if (follower != null)
            {
                _context.Followers.Remove(follower);
            }

            await _context.SaveChangesAsync();
        }

        public IQueryable<Follower> Query()
        {
            return _context.Followers;
        }
    }
}
