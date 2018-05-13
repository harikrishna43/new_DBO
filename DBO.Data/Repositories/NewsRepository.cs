using DBO.Common;
using DBO.Data.Models;
using DBO.Data.Utilities;
using DBO.Data.ViewModels;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using X.PagedList;

namespace DBO.Data.Repositories
{
    public class NewsRepository
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        public News GetById(int id)
        {
            return _db.News.Find(id);
        }

        /// <summary>
        /// Gets latest news of particular <see cref="Company"/>.
        /// </summary>
        /// <param name="companyId">Id of the <see cref="Company"/>.</param>
        /// <param name="currentCompanyId">Company id of the currently logged user.</param>
        /// <param name="hasMoreResults">Indicates if there are more results.</param>
        /// <param name="pageNumber">Number of the page.</param>
        /// <returns><see cref="IEnumerable{Company}"/>.</returns>
        public IEnumerable<NewsViewModel> GetNewsByCompanyId(int companyId, int currentCompanyId, out bool hasMoreResults, int pageNumber = 0)
        {
            hasMoreResults = false;
            //check if companies are connected, so visitor can put comments
            var connection = _db.Connections.FirstOrDefault(c => ((c.CompanyId1 == companyId && c.CompanyId2 == currentCompanyId)
                                                       || (c.CompanyId1 == currentCompanyId && c.CompanyId2 == companyId))
                                                       && c.Status == ConnectionStatus.Approved);

            var news = _db.News.Include(nameof(News.Comments))
                            .Include(nameof(News.Company))
                            .Include(nameof(News.Comments) + "." + nameof(Comment.User))
                            .Include(nameof(News.Comments) + "." + nameof(Comment.User) + "." + nameof(ApplicationUser.Company))
                            .Where(x => x.CompanyId == companyId)
                            .OrderByDescending(x => x.CreatedAt)
                            .Skip(Constants.NewsPageSize * pageNumber)
                            .Take(Constants.NewsPageSize + 1)
                            .ToList();

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
                    _db.Entry(newsItem).State = EntityState.Modified;
                }
            }

            _db.SaveChanges();


            //set indicator for comments and return news
            return news.Select(x => new NewsViewModel(x)
            {
                IsCommentAllowed = connection != null || companyId == currentCompanyId
            });
        }

        public IEnumerable<NewsViewModel> GetNewsFeed(string userId, string companyId, out bool hasMore, int pageNumber = 0, bool isAdmin = false)
        {
            hasMore = false;
            var connectedCompanies = new List<int>();
            var parsedCompanyId = -1;
            //prepare query
            var news = _db.News.Include(nameof(News.Comments))
                                .Include(nameof(News.Company))
                                .Include(nameof(News.User))
                                .Include("Comments.User")
                                .Include("Comments.User.Company")
                                .OrderByDescending(n => n.CreatedAt)
                                .Skip(Constants.NewsPageSize * pageNumber)
                                .Take(Constants.NewsPageSize + 1);

            //if user is logged in, check for connections and followings
            if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(userId) && !isAdmin)
            {
                //get connected companies
                parsedCompanyId = int.Parse(companyId);
                var connections = _db.Connections.Where(c => (c.CompanyId1 == parsedCompanyId || c.CompanyId2 == parsedCompanyId) && c.Status == ConnectionStatus.Approved)
                                                  .Select(c => new { c.CompanyId1, c.CompanyId2 });
                connectedCompanies.AddRange(connections.Where(c => c.CompanyId1 != parsedCompanyId).Select(c => c.CompanyId1));
                connectedCompanies.AddRange(connections.Where(c => c.CompanyId2 != parsedCompanyId).Select(c => c.CompanyId2));
                connectedCompanies.Add(parsedCompanyId);

                //get companies followed by current user
                var followedCompanies = new List<int>();
                var parsedUserId = Guid.Parse(userId);
                followedCompanies.AddRange(_db.Followers.Where(f => f.UserId == parsedUserId).Select(f => f.CompanyId));
                //update query
                news = news.Where(n => followedCompanies.Distinct().Contains(n.CompanyId) || connectedCompanies.Distinct().Contains(n.CompanyId));
            }

            var result = news.ToList();

            //check if there are more results
            if (result.Count > Constants.NewsPageSize)
            {
                result.RemoveAt(Constants.NewsPageSize);
                hasMore = true;
            }

            //Update view for each news
            foreach (var newsItem in result)
            {
                if (newsItem.CompanyId != parsedCompanyId)
                {
                    newsItem.Views++;
                    _db.Entry(newsItem).State = EntityState.Modified;
                }
            }
            _db.SaveChanges();

            //fetch all news from database
            return result.Select(n => new NewsViewModel(n)
            {
                IsCommentAllowed = connectedCompanies.Contains(n.CompanyId)
            }).ToList(); ;
        }

        /// <summary>
        /// Adding <see cref="Comment"/> to the <see cref="News"/>.
        /// </summary>
        /// <param name="comment"><see cref="CommentViewModel"/>.</param>
        /// <param name="userId">Id of the currently logged in user.</param>
        /// <returns><see cref="Task"/> for async execution.</returns>
        public async Task<Comment> AddComment(CommentViewModel comment, string userId)
        {
            var entity = new Comment
            {
                CreatedAt = DateTime.Now,
                UserId = userId,
                Content = comment.Content,
                NewsId = comment.NewsId
            };

            _db.Comments.Add(entity);
            await _db.SaveChangesAsync();
            return _db.Comments.OrderByDescending(c => c.Id)?.FirstOrDefault();
        }

        public async Task<News> CreateNews(NewsViewModel model, string userId, HttpPostedFileBase file = null)
        {
            var filePath = string.Empty;

            if (file != null)
            {

                var directoryPath = HttpContext.Current.Server.MapPath(Constants.NewsImagesPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                //upload file
                filePath = Path.Combine($"{model.CompanyId}{DateTime.Now.Millisecond}{Path.GetFileName(file.FileName)}");
                var fullPath = Path.Combine(directoryPath, filePath);
                FileUploader.UploadFile(file.InputStream, fullPath);
            }

            var news = new News
            {
                Title = model.Title,
                CreatedAt = DateTime.Now,
                Description = model.Content,
                ImagePath = filePath,
                UserId = userId,
                CompanyId = model.CompanyId
            };

            _db.News.Add(news);
            await _db.SaveChangesAsync();
            return _db.News.OrderByDescending(n => n.Id)?.FirstOrDefault();
        }

        public IEnumerable<NewsViewModel> ReadMoreNews(int pageNumber, out bool hasMore)
        {
            hasMore = false;

            var result = new List<NewsViewModel>();
            var pageSize = Constants.NewsPageSize;

            foreach (var news in _db.News.OrderBy(x => x.Id).Skip(pageNumber * pageSize).Take(pageSize + 1))
            {
                result.Add(new NewsViewModel(news));
            }

            if (result.Count > pageSize)
            {
                result.RemoveAt(pageSize);
                hasMore = true;
            }

            return result;
        }


    }
}