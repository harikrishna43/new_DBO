using DBO.Common;
using DBO.Data.Models;
using DBO.Data.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using X.PagedList;

namespace DBO.Data.Repositories
{
    public class CompanyRepository
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        private UserManager<ApplicationUser> _userManager;

        public CompanyRepository()
        {
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
        }

        public IQueryable<Company> Query()
        {
            return _db.Companies;
        }

        public List<CompanyViewModel> GetCompanies(string searchName, string searchCity, out bool hasMore, int pageNumber = 0)
        {
            hasMore = false;
            var query = _db.Companies.AsQueryable();
            if (!string.IsNullOrEmpty(searchName))
            {
                bool isNumber = int.TryParse(searchName, out var cvr);
                bool isPhoneNumber = Regex.Match(searchName, @"[0-9 ]+").Success;
                var skillIds = _db.Skills.Where(s => s.Name.Contains(searchName)).Select(s => s.Id).ToList();
                query = query.Where(c => c.Name.Contains(searchName)
                                         || (isNumber && c.CVR == cvr)
                                         || c.Email.Contains(searchName)
                                         || (isPhoneNumber && c.Phone.Contains(searchName))
                                         || c.CompanySkills.Any(s => skillIds.Contains(s.SkillId))
                );
            }

            if (!string.IsNullOrEmpty(searchCity))
            {
                query = query.Where(c => c.City.Contains(searchCity));
            }

            var result = new List<CompanyViewModel>();
            var pageSize = Common.Constants.CompaniesPageSize;
            foreach (var company in query.OrderBy(x => x.Id).Skip(pageNumber * pageSize).Take(pageSize + 1).ToList())
            {
                var viewModel = new CompanyViewModel(company);
                viewModel.Connections = _db.Connections.Count(x => (x.CompanyId1 == company.Id || x.CompanyId2 == company.Id) && x.Status == ConnectionStatus.Approved);

                result.Add(viewModel);
            }

            if (result.Count > pageSize)
            {
                result.RemoveAt(pageSize);
                hasMore = true;
            }

            return result;
        }

        /// <summary>
        /// get three companies = previous, current and next
        /// </summary>
        public IQueryable<Company> GetThreeCompanies(SearchParams searchParams, int n)
        {
            var query = GetSeaerchQuery(searchParams);
            var companies = n == 1
                ? query.OrderBy(x => x.Id).Take(2)
                : query.OrderBy(x => x.Id).Skip(n - 2).Take(3);
            return companies;
        }

        public List<Company> GetCompanySearch(SearchParams search, out bool hasMore, out int count, int pageNumber = 0)
        {
            hasMore = false;
            var query = GetSeaerchQuery(search);

            count = query.Count();

            var pageSize = Common.Constants.CompaniesPageSize;
            var result = query.OrderBy(x => x.Id).Skip(pageNumber * pageSize).Take(pageSize + 1).ToList();
            if (result.Count > pageSize)
            {
                result.RemoveAt(pageSize);
                hasMore = true;
            }

            return result;
        }

        private IQueryable<Company> GetSeaerchQuery(SearchParams search)
        {
            var query = _db.Companies.Include(c => c.CompanySkills).AsQueryable();
            if (!string.IsNullOrEmpty(search.SearchName))
            {
                bool isNumber = int.TryParse(search.SearchName, out var cvr);
                var skillIds = _db.Skills.Where(s => s.Name.Contains(search.SearchName)).Select(s => s.Id).ToList();
                query = query.Where(c => c.Name.Contains(search.SearchName)
                                         || (isNumber && c.CVR == cvr)
                                         || c.Email.Contains(search.SearchName)
                                         || c.Phone.Contains(search.SearchName)
                                         || c.CompanySkills.Any(s => skillIds.Contains(s.SkillId))
                );
            }

            if (search.SearchIndustryCode.HasValue && search.SearchIndustryCode > 0)
            {
                query = query.Where(c => c.IndustryId == search.SearchIndustryCode.Value);
            }

            if (!string.IsNullOrEmpty(search.SearchZipFrom))
            {
                query = !string.IsNullOrEmpty(search.SearchZipTo)
                    ? query.Where(c =>
                        c.PostCode.CompareTo(search.SearchZipFrom) >= 0 && c.PostCode.CompareTo(search.SearchZipTo) <= 0)
                    : query.Where(c => c.PostCode == search.SearchZipFrom);
            }

            if (search.OnlyUnprocessed)
            {
                query = query.Where(c => !c.IsProcessed);
            }
            return query;
        }

        public Select2PagedModel GetSelect2(int pageSize, int page, string term)
        {
            var result = new Select2PagedModel();

            var items = (from p in _db.Companies
                         where term == null || term == "" || p.Name.ToLower().Contains(term.ToLower())
                         orderby p.Id
                         select new Select2Model
                         {
                             id = p.Id.ToString(),
                             text = p.Name
                         }).ToPagedList(page, pageSize);

            result.TotalCount = items.TotalItemCount;
            result.Items = items.ToList();

            return result;
        }

        public CompanyNavigationViewModel GetCompany(SearchParams searchParams, int n)
        {
            var three = GetThreeCompanies(searchParams, n).ToList();
            if (three.Count == 0)
            {
                return new CompanyNavigationViewModel
                {
                    Previous = null,
                    Current = null,
                    Next = null
                };
            }

            if (n == 1)
            {
                if (three.Count == 1)
                {
                    return new CompanyNavigationViewModel
                    {
                        Previous = null,
                        Current = three[0].Id,
                        Next = null
                    };
                }

                return new CompanyNavigationViewModel
                {
                    Previous = null,
                    Current = three[0].Id,
                    Next = three[1].Id
                };
            }
            else if (three.Count == 2)
            {
                return new CompanyNavigationViewModel
                {
                    Previous = three[0].Id,
                    Current = three[1].Id,
                    Next = null
                };
            }
            else if (three.Count == 1)
            {
                return new CompanyNavigationViewModel
                {
                    Previous = null,
                    Current = three[1].Id,
                    Next = null
                };
            }

            var res = new CompanyNavigationViewModel()
            {
                Previous = three[0].Id,
                Current = three[1].Id,
                Next = three[2].Id
            };

            return res;
        }

        public Select2PagedModel GetSelect2IndustryCodes(int pageSize, int page, string term)
        {
            var result = new Select2PagedModel();
            var industryCodes = _db.Industries
                                             .Where(x => string.IsNullOrEmpty(term) || x.Name.Contains(term))
                                             .Select(x => new Select2Model
                                             {
                                                 id = x.Id.ToString(),
                                                 text = x.Name.ToString()
                                             })
                                             .OrderBy(x => x.id)
                                             .ToPagedList(page, pageSize);

            result.TotalCount = industryCodes.TotalItemCount;
            result.Items = industryCodes.ToList();

            return result;
        }

        public Company GetCompany(int id)
        {
            return _db.Companies.Find(id);
        }

        public Company GetUsersCompany(ApplicationUser user)
        {
            return this._db.Companies.First(e => e.Employees.Any(m => m.Id == user.Id));
        }

        public Company GetCompanyWithDetails(int id)
        {
            return _db.Companies
                .Include("Industry")
                .Include("Employees")
                .Include("CompanySkills")
                .Include("CompanySkills.Skill")
                .Include("RegistrationCodes")
                .Include("ScheduledEmails")
                .SingleOrDefault(x => x.Id == id);
        }

        public async Task UpdateCompany(Company company)
        {
            var savedCompany = _db.Companies.Find(company.Id);
            if (savedCompany != null)
            {
                _db.Entry(savedCompany).CurrentValues.SetValues(company);
                await _db.SaveChangesAsync();
            }
        }

        public void Delete(Company company)
        {
            _db.Companies.Remove(company);
            _db.SaveChanges();
        }

        public void Add(Company company)
        {
            _db.Companies.Add(company);
            _db.SaveChanges();
        }

        public List<SelectListItem> GetIndustryList(int? id = null)
        {
            var selectListItems = _db.Industries
                .Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString(), Selected = i.Id == id })
                .ToList();

            var defaultItem = new SelectListItem { Value = "", Text = Utilities.ResourceString.Instance.Select_industry as string };
            if (id == null)
            {
                defaultItem.Selected = true;
            }
            else
            {
                defaultItem.Text = "";
            }

            selectListItems.Insert(0, defaultItem);
            return selectListItems;
        }

        public async Task UpdateAddress(int id, string city, string address, string postCode)
        {
            var company = await _db.Companies.FindAsync(id);

            if (company != null)
            {
                company.City = city;
                company.Address = address;
                company.PostCode = postCode;

                await _db.SaveChangesAsync();
            }
        }

        public async Task UploadLogo(int id, HttpPostedFileBase file)
        {
            var company = _db.Companies.Find(id);

            if (company != null)
            {
                var directoryPath = HttpContext.Current.Server.MapPath(Common.Constants.CompanyLogosPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var filePath = Path.Combine($"{id}{DateTime.Now.Millisecond}{Path.GetFileName(file.FileName)}");
                var fullPath = Path.Combine(HttpContext.Current.Server.MapPath(Common.Constants.CompanyLogosPath), filePath);

                UploadFile(file.InputStream, fullPath);

                company.Image = filePath;
                await _db.SaveChangesAsync();
            }
        }

        private static void UploadFile(Stream image, string filePath)
        {
            using (var file = File.Create(filePath))
            {
                image.Seek(0, SeekOrigin.Begin);
                image.CopyTo(file);
            }
        }

        public async Task<bool> EmailInUseByOtherCompany(string email, int id)
        {
            return await _db.Companies.AnyAsync(c => c.Email == email && c.Id != id);
        }

        public bool CheckIfCompanyIsClaimed(int id)
        {
            var companyUser = _userManager.Users.AsQueryable().Include("Company").ToList()
                .Where(x => _userManager.IsInRole(x.Id, Common.Constants.CompanyRole))
                .FirstOrDefault(x => x.CompanyId.HasValue && x.CompanyId == id);
            return companyUser != null;
        }
    }
}
