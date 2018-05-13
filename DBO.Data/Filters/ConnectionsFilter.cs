using DBO.Data.Models;

using System.Linq;

namespace DBO.Data.Filters
{
    public static class ConnectionsFilter
    {
        public static bool AreCompaniesConnectionApproved(this IQueryable<Connection> connections, int companyId, int connectedCompanyId)
        {
            return connections.Any(c => ((c.CompanyId1 == companyId && c.CompanyId2 == connectedCompanyId)
                                        || (c.CompanyId1 == connectedCompanyId && c.CompanyId2 == companyId))
                                        && c.Status == ConnectionStatus.Approved);
        }
    }
}
