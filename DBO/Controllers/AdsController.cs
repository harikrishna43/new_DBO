using AutoMapper;

using DBO.Common;
using DBO.Common.Helpers;
using DBO.Data.Models;
using DBO.Data.Utilities;
using DBO.Data.ViewModels;
using DBO.Services.Contract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace DBO.Controllers
{
    [Authorize(Roles = Constants.CompanyRole + ", " + Constants.AdminRole)]
    public class AdsController : BaseController
    {
        private readonly IAdsService _adsService;
        private readonly ISubdataService _subdataService;

        public AdsController(IAdsService adsService, ISubdataService subdataService)
        {
            _adsService = adsService;
            _subdataService = subdataService;
        }

        public ActionResult Index()
        {
            var isAdmin = User.IsInRole(Constants.AdminRole);
            var model = Mapper.Map<List<AdvertisementViewModel>>(isAdmin
                ? _adsService.GetAll()
                : _adsService.GetAllForUser(CurrentUserId));

            return View(model);
        }

        public ActionResult Create()
        {
            PopulateDropDownListAndKeys();
            return View(new AdvertisementViewModel
            {
                UserId = CurrentUserId.ToString(),
                CompanyId = CurrentUserCompanyId,
                IsAdmin = User.IsInRole(Constants.AdminRole)
            });
        }

        [HttpPost] //ValidateAntiForgeryToken
        public ActionResult Create(AdvertisementViewModel model)
        {
            if (TempData["NeedPayment"] != null)
                TempData["NeedPayment"] = null;

            if (!ModelState.IsValid)
            {
                PopulateDropDownListAndKeys(model);
                return View(model);
            }
            if (!model.IsAdmin && !model.IsPaid)
            {
                TempData["NeedPayment"] = true;
                PopulateDropDownListAndKeys(model);
                return View(model);
            }

            var ad = Mapper.Map<Advertisement>(model);
            ad.UserId = CurrentUserId.ToString();
            ad.CreatedByAdmin = model.IsAdmin;
            _adsService.Create(ad);

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Edit(int id)
        {
            var model = Mapper.Map<AdvertisementViewModel>(_adsService.GetById(id, CurrentUserId));
            model.IsAdmin = User.IsInRole(Constants.AdminRole);
            model.CompanyId = CurrentUserCompanyId;
            PopulateDropDownListAndKeys(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AdvertisementViewModel model)
        {
            if (model.UserId != CurrentUserId.ToString())
            {
                return new HttpUnauthorizedResult();
            }

            ModelState.Remove(nameof(AdvertisementViewModel.Budget));
            ModelState.Remove(nameof(AdvertisementViewModel.ClickPrice));
            if (!ModelState.IsValid)
            {
                PopulateDropDownListAndKeys(model);
                return View(model);
            }

            var ad = Mapper.Map<Advertisement>(model);
            ad.CreatedByAdmin = User.IsInRole(Constants.AdminRole);

            _adsService.Update(ad);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult Overview(int id)
        {
            var ad = _adsService.GetById(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            ViewBag.Reason = _adsService.CheckAdsVisibilityForCurrentUser(ad);
            return View(Mapper.Map<AdvertisementViewModel>(ad));
        }

        public ActionResult Remove(int id)
        {
            _adsService.Remove(id, CurrentUserId);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Open(int id)
        {
            var ip = Request.UserHostAddress;
            var model = _adsService.OpenAd(id, CurrentUserId.ToString(), ip);
            if (!string.IsNullOrEmpty(model.Link))
            {
                var link = LinkHelper.GetCorrectLink(model.Link);
                return Redirect(link);
            }
            else
            {
                var companyId = model.User.CompanyId;
                if (companyId.HasValue)
                    return RedirectToAction("Details", "Business", new { id = companyId.Value });
                else
                    return RedirectToAction("Index", "Home");
            }
        }


        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult DisplayAds(string viewName = "DisplayAds_Bottom")
        {
            var isUserLoggedIn = Request.IsAuthenticated;
            var isUserCompany = User.IsInRole(Constants.CompanyRole);
            var companyId = -1;

            if (isUserLoggedIn && isUserCompany)
            {
                companyId = CurrentUserCompanyId;
            }

            ViewBag.Ads = Mapper.Map<IEnumerable<DisplayAdViewModel>>(_adsService.DisplayAds(12, companyId));

            return PartialView(viewName);
        }

        [HttpGet]
        public ActionResult TransferBudget(int id, decimal? remainingBudget = null)
        {
            var isAdmin = User.IsInRole(Constants.AdminRole);
            var model = new TransferBudgetViewModel { CurrentAdId = id, RemainingBudget = remainingBudget };
            PopulateDropDownListForTransfer(isAdmin, id);
            return PartialView("_TransferBudget", model);
        }

        [HttpPost] //ValidateAntiForgeryToken
        public ActionResult TransferBudget(TransferBudgetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var isAdmin = User.IsInRole(Constants.AdminRole);
                PopulateDropDownListForTransfer(isAdmin, model.CurrentAdId);
                return PartialView("_TransferBudget", model);
            }

            _adsService.TransferBudget(model.CurrentAdId, model.SelectedAdvertisementId.Value,
                model.Amount.Value, model.TransferToAnother, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                var isAdmin = User.IsInRole(Constants.AdminRole);
                PopulateDropDownListForTransfer(isAdmin, model.CurrentAdId);
                model.ErrorMessage = errorMessage;
                return PartialView("_TransferBudget", model);
            }

            return Json(new { Success = true });
        }

        [HttpPost]
        public ActionResult StopAd(int id)
        {
            var remainingBudget = _adsService.StopAdvertisement(id);
            if (remainingBudget > decimal.Zero)
                return RedirectToAction(nameof(TransferBudget), new { id, remainingBudget });
            else
                return Json(new { NoRemainingBudget = true });
        }

        [NonAction]
        private void PopulateDropDownListForTransfer(bool isAdmin, int id)
        {
            var advertisementList = isAdmin
                ? Mapper.Map<List<AdvertisementViewModel>>(_adsService.GetAll())
                : Mapper.Map<List<AdvertisementViewModel>>(_adsService.GetAllForUser(CurrentUserId));

            ViewBag.ListOfAds = new List<SelectListItem>(advertisementList?
                .Where(c => c.Id != id)?
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Headline
                }));
        }

        private void PopulateDropDownListAndKeys(AdvertisementViewModel model = null)
        {
            ViewBag.Skills = new List<SelectListItem>(_subdataService.GetSkills()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                    Selected = model?.SelectedSkills != null && model.SelectedSkills.Any(y => x.Id.ToString() == y)
                }));

            ViewBag.Industries = new List<SelectListItem>(_subdataService.GetIndustries()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                    Selected = model?.SelectedIndustries != null && model.SelectedIndustries.Any(y => x.Id.ToString() == y)
                }));
            ViewBag.AdvertisementTypes = new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = ResourceString.Instance.Image,
                    Value = ((int)AdvertisementType.Image).ToString(),
                    Selected = model?.Type == AdvertisementType.Image
                },
                new SelectListItem
                {
                    Text = ResourceString.Instance.Video,
                    Value = ((int)AdvertisementType.Video).ToString(),
                    Selected = model?.Type == AdvertisementType.Video
                },
            };

            ViewBag.LocationType = new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = ResourceString.Instance.WithoutLocation,
                    Value = ((int)LocationType.WithoutLocation).ToString(),
                    Selected =  model?.LocationType == LocationType.WithoutLocation
                },
                new SelectListItem
                {
                    Text = ResourceString.Instance.Country,
                    Value = ((int)LocationType.Country).ToString(),
                    Selected = model?.LocationType == LocationType.Country
                },
                new SelectListItem
                {
                    Text = ResourceString.Instance.Cities,
                    Value = ((int)LocationType.Cities).ToString(),
                    Selected = model?.LocationType == LocationType.Cities
                },
            };

            ViewBag.CountriesList = CountriesHelper.GetCountriesList()?.Select(c => new SelectListItem
            {
                Text = c,
                Value = c,
                Selected = model?.Location?.Equals(c) == true
            });

            //API KEYS
            ViewBag.GoogleApiKey = ConfigurationManager.AppSettings["GOOGLEMAPS_API_KEY"];
            ViewBag.PaylikePublicKey = ConfigurationManager.AppSettings["PAYLIKE_PUBLIC_KEY"];
        }
    }
}