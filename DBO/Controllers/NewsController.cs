using DBO.Data.Models;
using DBO.Data.Repositories;
using DBO.Data.ViewModels;
using DBO.Extensions;
using DBO.Services.Contract;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using static DBO.Common.Constants;

namespace DBO.Controllers
{
    public class NewsController : BaseController
    {
        private readonly INewsService _newsService;
        private readonly NewsRepository _newsRepository;
        private readonly INotificationService _notificationService;

        public NewsController(INewsService newsService, INotificationService notificationService)
        {
            _newsService = newsService;
            _newsRepository = new NewsRepository();
            _notificationService = notificationService;
        }

        [HttpGet]
        public PartialViewResult GetNewsForCompany(int companyId, int pageNumber = 0)
        {
            var currentCompanyId = -1;
            if (!User.IsInRole(AdminRole) && !string.IsNullOrEmpty(User.GetClaimValue(CompanyIdClaim)))
            {
                currentCompanyId = int.Parse(User.GetClaimValue(CompanyIdClaim));
            }

            var model = _newsService.GetNewsByCompanyId(companyId, currentCompanyId, out var hasMoreResults, pageNumber);
            ViewBag.HasMoreResults = hasMoreResults;
            Response.Headers.Add("X-HasMoreResults", hasMoreResults.ToString());

            return PartialView("News", model);
        }

        [HttpPost]
        [Authorize(Roles = CompanyRole + ", " + EmployeeRole)]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateNews(NewsViewModel viewModel, HttpPostedFileBase file,
            string redirectAction, string redirectController)
        {
            if (ModelState.IsValid)
            {
                var userId = User.GetClaimValue(UserIdClaim);
                var createdNews = await _newsRepository.CreateNews(viewModel, userId, file);
                _notificationService.Create(NotificationType.Bells, EventType.NewsCreated, createdNews.Id);
            }

            if (!string.IsNullOrEmpty(redirectAction) && !string.IsNullOrEmpty(redirectController))
            {
                return RedirectToAction(redirectAction, redirectController);
            }

            return RedirectToAction("Details", "Business", new { id = viewModel.CompanyId });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> AddComment(CommentViewModel comment, int companyId,
                                                   string redirectAction, string redirectController)
        {
            if (ModelState.IsValid)
            {
                var userId = User.GetClaimValue(UserIdClaim);
                var createdComment = await _newsRepository.AddComment(comment, userId);
                _notificationService.Create(NotificationType.Bells, EventType.CommentCreated, createdComment.Id);
            }

            if (!string.IsNullOrEmpty(redirectAction) && !string.IsNullOrEmpty(redirectController))
            {
                return RedirectToAction(redirectAction, redirectController);
            }
            else
            {
                return RedirectToAction("Details", "Business", new { id = companyId });
            }
        }
    }
}