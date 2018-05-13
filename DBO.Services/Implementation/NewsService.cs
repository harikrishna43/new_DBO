using DBO.Common;
using DBO.Data.Filters;
using DBO.Data.Models;
using DBO.Data.Repositories.Contract;
using DBO.Data.ViewModels;
using DBO.Services.Contract;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DBO.Services.Implementation
{
    public class NewsService : INewsService
    {
        private readonly IRepository<News> _newsRepository;
        private readonly IRepository<Connection> _connectionsRepository;

        public NewsService(IRepository<News> newsRepository, IRepository<Connection> connectionsRepository)
        {
            _newsRepository = newsRepository;
            _connectionsRepository = connectionsRepository;
        }

        public IEnumerable<NewsViewModel> GetNewsByCompanyId(int companyId, int currentCompanyId, out bool hasMoreResults, int pageNumber = 0)
        {
            hasMoreResults = false;
            var areCompaniesConnected = false;

            //include associated properties
            var query = _newsRepository.Query().Include(nameof(News.Comments))
                                              .Include(nameof(News.Company))
                                              .Include(nameof(News.User))
                                              .Include("Comments.User")
                                              .Include("Comments.User.Company");

            //set filter
            query = query.Where(x => x.CompanyId == companyId);

            //set pagination
            query = query.OrderByDescending(x => x.CreatedAt)
                         .Skip(Constants.NewsPageSize * pageNumber)
                         .Take(Constants.NewsPageSize + 1);

            //execute query
            var news = query.ToList();
            if (news.Any())
            {
                if (news.Count > Constants.NewsPageSize)
                {
                    news.RemoveAt(Constants.NewsPageSize);
                    hasMoreResults = true;
                }

                //if company is not viewing profile page, increment view of the news item
                foreach (var newsItem in news)
                {
                    if (currentCompanyId != companyId)
                    {
                        newsItem.Views++;
                        _newsRepository.Update(newsItem);
                    }
                }

                _newsRepository.SaveChanges();

                //check if companies are connected, so visitor can put comments
                areCompaniesConnected = _connectionsRepository.Query().AreCompaniesConnectionApproved(companyId, currentCompanyId);
            }

            //set indicator for comments and return news
            return news.Select(x => new NewsViewModel(x)
            {
                IsCommentAllowed = areCompaniesConnected || companyId == currentCompanyId
            });
        }
    }
}
