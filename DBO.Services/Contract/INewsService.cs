using DBO.Data.ViewModels;

using System.Collections.Generic;

namespace DBO.Services.Contract
{
    public interface INewsService
    {
        IEnumerable<NewsViewModel> GetNewsByCompanyId(int companyId, int currentCompanyId, out bool hasMoreResults, int pageNumber = 0);
    }
}
