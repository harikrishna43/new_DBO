using DBO.Data.Models;
using DBO.Data.Repositories.Contract;
using DBO.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBO.Services.Implementation
{
    public class FollowerService : IFollowersService
    {
        private IRepository<Follower> _followerRepository;

        public FollowerService(IRepository<Follower> followerRepository)
        {
            _followerRepository = followerRepository;
        }
        public async Task CreateAsync(int companyId, Guid userId)
        {
            var follower = new Follower
            {
                CompanyId = companyId,
                UserId = userId
            };

            _followerRepository.Create(follower);
            _followerRepository.SaveChanges();
        }

        public async Task DeleteAsync(int companyId, Guid userId)
        {
            var follower = _followerRepository.Query().FirstOrDefault(x => x.CompanyId == companyId && x.UserId == userId);

            if(follower != null)
            {
                _followerRepository.Remove(follower.Id);
                _followerRepository.SaveChanges();
            }
        }

        public Follower Get(int companyId, Guid userId)
        {
            return _followerRepository.Query().FirstOrDefault(x => x.CompanyId == companyId && x.UserId == userId);
        }
    }
}
