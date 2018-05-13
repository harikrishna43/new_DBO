using DBO.Data.Models;
using DBO.Data.Repositories;
using DBO.Extensions;
using DBO.Services.Contract;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DBO.Controllers
{
    [Authorize]
    public class ConnectionsController : Controller
    {
        private ConnectionRepository _connectionRepository = new ConnectionRepository();
        private IFollowersService _followersService;
        private INotificationService _notificationService;

        public ConnectionsController(IFollowersService followersService, INotificationService notificationService)
        {
            _followersService = followersService;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<ActionResult> Connect(int companyId)
        {
            if (int.TryParse(User.GetClaimValue(Common.Constants.CompanyIdClaim), out var connectedCompanyId))
            {
                var connection = _connectionRepository.Get(companyId, connectedCompanyId);

                if (connection != null)
                {
                    if (connection.Status == ConnectionStatus.Rejected)
                    {
                        connection.Status = ConnectionStatus.Requested;
                        await _connectionRepository.UpdateAsync(connection);
                        _notificationService.Create(NotificationType.Email, EventType.ConnectionRequest, connection.Id);
                    }
                }
                else
                {
                    connection = await _connectionRepository.CreateAsync(companyId, connectedCompanyId);
                    _notificationService.Create(NotificationType.Email, EventType.ConnectionRequest, connection.Id);
                }
            }

            return RedirectToAction("Details", "Business", new { id = companyId });
        }

        [HttpGet]
        public async Task<ActionResult> Disconnect(int companyId)
        {
            var connectedCompanyId = User.GetClaimValue(Common.Constants.CompanyIdClaim);
            await _connectionRepository.DeleteAsync(companyId, int.Parse(connectedCompanyId));
            return RedirectToAction("Details", "Business", new { id = companyId });
        }

        [HttpGet]
        public async Task<ActionResult> AcceptConnection(int companyId)
        {
            var currentCompanyId = int.Parse(User.GetClaimValue(Common.Constants.CompanyIdClaim));
            var connection = _connectionRepository.Get(currentCompanyId, companyId);

            if (connection != null)
            {
                connection.Status = ConnectionStatus.Approved;
                await _connectionRepository.UpdateAsync(connection);
                _notificationService.Create(NotificationType.Email, EventType.ConnectionApprove, connection.Id);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Details", "Business", new { id = currentCompanyId });
        }

        [HttpGet]
        public async Task<ActionResult> DeclineConnection(int connectionId)
        {
            var connection = _connectionRepository.Get(connectionId);

            if (connection != null)
            {
                connection.Status = ConnectionStatus.Rejected;
                await _connectionRepository.UpdateAsync(connection);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Details", "Business", new { id = connection.CompanyId1 });
        }


        [HttpGet]
        public async Task<ActionResult> Follow(int companyId)
        {
            var userId = Guid.Parse(User.GetClaimValue("Id"));
            await _followersService.CreateAsync(companyId, userId);
            var follower = _followersService.Get(companyId, userId);
            _notificationService.Create(NotificationType.Email, EventType.NewFollower, follower.Id);
            return RedirectToAction("Details", "Business", new { id = companyId });

        }

        [HttpGet]
        public async Task<ActionResult> Unfollow(int companyId)
        {
            var userId = Guid.Parse(User.GetClaimValue("Id"));
            await _followersService.DeleteAsync(companyId, userId);

            return RedirectToAction("Details", "Business", new { id = companyId });
        }

        #region Child Actions

        [ChildActionOnly, AllowAnonymous]
        public ActionResult CheckForConnection(int companyId)
        {
            var partialName = "_ConnectButtonPartial";
            var role = User.GetClaimValue(x => x.Type.Contains("role"));

            if (role != Common.Constants.CompanyRole)
            {
                return Content(string.Empty);
            }

            if (int.TryParse(User.GetClaimValue(Common.Constants.CompanyIdClaim), out var connectedCompanyId))
            {
                if (companyId == connectedCompanyId)
                {
                    return Content(string.Empty);
                }

                var connection = _connectionRepository.Query().FirstOrDefault(x => (x.CompanyId1 == companyId && x.CompanyId2 == connectedCompanyId) ||
                                                                          (x.CompanyId1 == connectedCompanyId && x.CompanyId2 == companyId));
                if (connection != null)
                {
                    switch (connection.Status)
                    {
                        case ConnectionStatus.Approved:
                            partialName = "_ConnectedButtonPartial";
                            break;
                        case ConnectionStatus.Requested:
                            partialName = "_WaitingForApprovalButtonPartial";
                            break;
                        case ConnectionStatus.Rejected:
                            return Content(string.Empty);
                        default:
                            partialName = "_ConnectButtonPartial";
                            break;
                    }
                }
            }

            return PartialView($"Buttons/{partialName}", companyId);
        }

        [ChildActionOnly, AllowAnonymous]
        public ActionResult CheckForFollower(int companyId)
        {
            var partialName = "_FollowButtonPartial";

            int.TryParse(User.GetClaimValue(Common.Constants.CompanyIdClaim), out var loggedInCompanyId);
            var role = User.GetClaimValue(x => x.Type.Contains("role"));

            if (role == Common.Constants.AdminRole
                || companyId == loggedInCompanyId
                || !Guid.TryParse(User.GetClaimValue("Id"), out var userId))
            {
                return Content(string.Empty);
            }

            var follower = _followersService.Get(companyId, userId);

            if (follower != null)
            {
                partialName = "_FollowingButtonPartial";
            }

            return PartialView($"Buttons/{partialName}", companyId);
        }

        #endregion
    }
}