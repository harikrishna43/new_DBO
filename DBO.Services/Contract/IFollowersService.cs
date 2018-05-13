using DBO.Data.Models;
using System;
using System.Threading.Tasks;

namespace DBO.Services.Contract
{
    public interface IFollowersService
    {
        Follower Get(int companyId, Guid userId);
        Task CreateAsync(int companyId, Guid userId);
        Task DeleteAsync(int companyId, Guid userId);
    }
}
