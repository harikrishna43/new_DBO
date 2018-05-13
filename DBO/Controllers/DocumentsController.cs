using DBO.Common;
using DBO.Data.Models;
using DBO.Data.Repositories;
using DBO.Data.Utilities;

using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DBO.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class DocumentsController : Controller
    {
        private readonly DocumentRepository _documentRepository = new DocumentRepository();
        private readonly LanguageRepository _languageRepository = new LanguageRepository();

        // GET: Templates
        public async Task<ActionResult> Index()
        {
            return View(await _documentRepository.GetAll());
        }

        // GET: Templates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Templates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Id,Content")] Document document)
        {
            if (ModelState.IsValid)
            {
                //template.Content = HttpUtility.HtmlEncode(template.Content);
                //db.Documents.Add(template);
                //db.SaveChanges();
                //return RedirectToAction("Index");
            }

            return View(document);
        }

        // GET: Templates/Edit/5
        public async Task<ActionResult> Edit(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var template = await _documentRepository.GetByName(name);

            if (template == null)
            {
                return HttpNotFound();
            }

            ViewBag.Languages = await _languageRepository.GetAll();
            return View(template);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(Document document)
        {
            if (ModelState.IsValid)
            {
                await _documentRepository.Update(document.Id, document.Content, document.Name, document.LanguageId);
                return RedirectToAction(nameof(Index));
            }

            return View(document);
        }
    }
}
