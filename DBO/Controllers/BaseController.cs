using DBO.Extensions;
using Microsoft.AspNet.Identity;
using System;
using System.Web.Mvc;

namespace DBO.Controllers
{
    public class BaseController : Controller
    {
        protected virtual Guid CurrentUserId
        {
            get
            {
                Guid userId = Guid.Empty;
                Guid.TryParse(User.GetClaimValue(Common.Constants.UserIdClaim), out userId);
                if (userId == Guid.Empty)
                    Guid.TryParse(User.Identity.GetUserId(), out userId);
                return userId;
            }
        }

        protected virtual int CurrentUserCompanyId
        {
            get
            {
                int.TryParse(User.GetClaimValue(Common.Constants.CompanyIdClaim), out var companyId);
                return companyId;
            }
        }

    }
}