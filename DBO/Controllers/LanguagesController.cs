using DBO.Common;
using DBO.Data.Models;
using DBO.Data.Repositories;
using DBO.Data.Utilities;
using DBO.Data.ViewModels;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DBO.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class LanguagesController : Controller
    {
        private readonly LanguageRepository _languageRepository = new LanguageRepository();

        // GET: Languages
        public async Task<ActionResult> Index()
        {
            var languages = (await _languageRepository.GetAll()).Select(x => new LanguageViewModel(x));
            return View(languages);
        }

        // GET: Languages/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var language = await _languageRepository.Get(id.Value);

            if (language == null)
            {
                return HttpNotFound();
            }

            return View(language);
        }

        // GET: Languages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Languages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(LanguageViewModel language)
        {
            if (ModelState.IsValid)
            {
                await _languageRepository.Create(language.Name);
                return RedirectToAction(nameof(Index));
            }

            return View(language);
        }

        // POST: Languages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(LanguageViewModel language)
        {
            if (ModelState.IsValid)
            {
                await _languageRepository.Update(language.Id, language.Name);
                return Content(ResourceString.Instance.SuccesfullySaved);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ResourceString.Instance.ValidateYourInput);
        }

        // GET: Languages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            await _languageRepository.Delete(id.Value);
            return RedirectToAction(nameof(Index));
        }
    }
}
