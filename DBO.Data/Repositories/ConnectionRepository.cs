using DBO.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBO.Data.Repositories
{
    public class ConnectionRepository
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public ConnectionRepository()
        {

        }
        public IQueryable<Connection> GetAll()
        {
            return _context.Connections.AsQueryable();
        }

        public async Task<Connection> CreateAsync(int companyId, int connectedCompanyId)
        {
            var connection = new Connection
            {
                CompanyId1 = companyId,
                CompanyId2 = connectedCompanyId,
                Status = ConnectionStatus.Requested

            };
            _context.Connections.Add(connection);
            await _context.SaveChangesAsync();
            return _context.Connections.OrderByDescending(c => c.Id)?.FirstOrDefault();
        }

        public async Task DeleteAsync(int companyId, int connectedCompanyId)
        {
            var connection = Query().FirstOrDefault(x => (x.CompanyId1 == companyId && x.CompanyId2 == connectedCompanyId) ||
                                                (x.CompanyId1 == connectedCompanyId && x.CompanyId2 == companyId));

            if (connection != null)
            {
                _context.Connections.Remove(connection);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Connection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            _context.Entry(connection).State = System.Data.Entity.EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public Connection Get(int companyId, int connectedCompanyId)
        {
            return _context.Connections.FirstOrDefault(x => x.CompanyId1 == companyId && x.CompanyId2 == connectedCompanyId);
        }

        public Connection Get(int connectionId)
        {
            return _context.Connections.FirstOrDefault(x => x.Id == connectionId);
        }

        public List<Connection> GetPendingConnections(int companyId)
        {
            return _context.Connections
                           .Include(nameof(Connection.Company2))
                           .Where(x => x.CompanyId1 == companyId && x.Status == ConnectionStatus.Requested)
                           .ToList();
        }

        public IQueryable<Connection> Query()
        {
            return _context.Connections;
        }
    }
}
