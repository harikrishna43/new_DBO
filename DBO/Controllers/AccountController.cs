using DBO.Data;
using DBO.Data.Models;
using DBO.Data.Repositories;
using DBO.Data.Utilities;
using DBO.Data.ViewModels;
using DBO.Extensions;
using DBO.Models;
using DBO.Services.Email;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DBO.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly CompanyRepository _companyRepository = new CompanyRepository();

        public AccountController()
        {
            
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnToAdmin = returnUrl != null && returnUrl.Equals("/admin");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = UserManager.Find(model.Email, model.Password);
            //user = UserManager.FindByEmailAsync(model.Email).Result;
            if (user != null)
            {
                var identity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                identity.AddClaimsForUser(user);

                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = model.RememberMe, RedirectUri = returnUrl }, identity);

                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", ResourceString.Instance.InvalidLoginAttempt);
            }
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        private bool IsCompanyValid(CreateCompanyViewModel model)
        {
            return !GetAllDuplicateCompanies(model).Any();
        }

        private IEnumerable<Company> GetAllDuplicateCompanies(CreateCompanyViewModel model)
        {
            return db.Companies.Where(x => (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(model.Name)) ||
                                            (!string.IsNullOrEmpty(x.Email) && x.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)) ||
                                            (!string.IsNullOrEmpty(x.Phone) && x.Phone.Equals(model.Phone, StringComparison.OrdinalIgnoreCase))
                                          );
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(CreateCompanyViewModel model)
        {
            if (ModelState.IsValid)
            {
                //validate
                var isCompanyRegistration = model.Role.Equals(Common.Constants.CompanyRole);
                if (isCompanyRegistration && !IsCompanyValid(model))
                {
                    var duplicateCompany = GetAllDuplicateCompanies(model).FirstOrDefault();
                    var errorMessage = string.Format(ResourceString.Instance.CompanyAlreadyExists,
                        !string.IsNullOrEmpty(duplicateCompany.Email) ? duplicateCompany.Email : "",
                        !string.IsNullOrEmpty(duplicateCompany.Name) ? duplicateCompany.Name : "");

                    ViewBag.DuplicateId = duplicateCompany.Id;
                    ViewBag.IsCompanyClaimed = _companyRepository.CheckIfCompanyIsClaimed(duplicateCompany.Id);
                    ModelState.AddModelError(string.Empty, errorMessage);

                    return View(model);
                }

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var password = Guid.NewGuid().ToString().Substring(0, 8) + "!1jK";
                var result = await UserManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    string subject, body;
                    await UserManager.AddToRoleAsync(user.Id, model.Role);

                    if (isCompanyRegistration)
                    {
                        var newCompany = new Company
                        {
                            Name = model.Name,
                            Email = model.Email,
                            Phone = model.Phone
                        };

                        db.Companies.Add(newCompany);
                        db.SaveChanges();
                        user.CompanyId = newCompany.Id;

                        await UserManager.UpdateAsync(user);

                        subject = "Company registered";
                        body = "Your company is registered. Your account password is " + password;
                    }
                    else
                    {
                        subject = "Account registered";
                        body = "Your account is registered. Your account password is " + password;
                    }

                    var emailService = new GoogleEmailService(
                        model.Email,
                        subject,
                        model.Name,
                        body,
                        true,
                        false
                    );
                    emailService.SendMail();

                    await UserManager.AddDboClaims(user);
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    if (isCompanyRegistration) return RedirectToAction("BasicData", "Home", new { id = user.CompanyId, message = "Thank you for registre your company, please take some time to fill out your company profile." });

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        ////[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            if (Session["ReturnToAdminRole"] != null && (bool)Session["ReturnToAdminRole"] == true)
            {
                Session["ReturnToAdminRole"] = null;
                LoginAsAdmin();
                return RedirectToAction("Index", "Companies");
            }

            return RedirectToAction("Index", "Home");
        }

        private void LoginAsAdmin()
        {
            var user = UserManager.FindByEmail(Common.Constants.AdminEmail);
            if (user != null)
            {
                var identity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                identity.AddClaimsForUser(user);

                AuthenticationManager.SignIn(identity);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}