using DBO.Data.Models;
using DBO.Data.Repositories.Contract;

namespace DBO.Data.Repositories.Implementation
{
    public class AdsRepository : Repository<Advertisement>, IAdsRepository
    {
        public AdsRepository(ApplicationDbContext context) : base(context) { }
    }
}
