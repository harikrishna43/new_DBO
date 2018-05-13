using DBO.Common;
using DBO.Data;
using DBO.Data.Repositories;
using DBO.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBO.Data.Models;
using System.Threading.Tasks;
using System.Net;
using DBO.Data.Utilities;

namespace DBO.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class TranslationsController : Controller
    {
        private TranslationRepository _translationRepository = new TranslationRepository();
        private LanguageRepository _languageRepository = new LanguageRepository();

        // GET: Translations
        public async Task<ActionResult> Index()
        {
            var translations = await _translationRepository.GetAllGroupedByName();

            return View(translations);
        }

        //public ActionResult Delete(int engId, int daId)
        //{
        //    _translationRepository.Delete(engId, daId);
        //    _translationRepository.Save();
        //    return RedirectToAction(nameof(Index));
        //}


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Resource translation)
        {
            return RedirectToAction(nameof(Create));
        }

        public async Task<ActionResult> Edit(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return HttpNotFound();
            }

            ViewBag.Languages = await _languageRepository.GetAll();
            return View(await _translationRepository.GetByName(name));
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ResourceViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _translationRepository.Update(model.Id, model.Name, model.Value, model.LanguageId);
                return Content((string)ResourceString.Instance.SuccessfullySaved);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, (string)ResourceString.Instance.ValidateYourInput);
        }
    }
}