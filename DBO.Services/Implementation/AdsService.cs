using DBO.Common;
using DBO.Data.Models;
using DBO.Data.Repositories.Contract;
using DBO.Data.Utilities;
using DBO.Services.Contract;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace DBO.Services.Implementation
{
    public class AdsService : IAdsService
    {
        private readonly IRepository<Advertisement> _adsRepository;
        private readonly IRepository<AdsSkills> _adsSkillsRepository;
        private readonly IRepository<AdsIndustries> _adsIndustryRepository;
        private readonly IRepository<AdClick> _adClicksRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<LogItem> _logItemRepositoty;


        public AdsService(IRepository<Advertisement> adsRepository,
                          IRepository<AdsSkills> adsSkillsRepository,
                          IRepository<AdsIndustries> adsIndustryRepository,
                          IRepository<AdClick> adClicksRepository,
                          IRepository<Company> companyRepository,
                          IRepository<LogItem> logItemRepositoty)
        {
            _adsRepository = adsRepository;
            _adsSkillsRepository = adsSkillsRepository;
            _adsIndustryRepository = adsIndustryRepository;
            _adClicksRepository = adClicksRepository;
            _companyRepository = companyRepository;
            _logItemRepositoty = logItemRepositoty;
        }

        private void AddLogItem(string message)
        {
            _logItemRepositoty.Create(new LogItem
            {
                Time = DateTime.Now,
                Value = message
            });
            _logItemRepositoty.SaveChanges();
        }

        public Advertisement Create(Advertisement model)
        {
            //TODO: move image manipulation to centralized place
            model.FilePath = UploadFile(model.FilePath, model.Type);

            var entity = _adsRepository.Create(model);
            _adsRepository.SaveChanges();

            foreach (var skill in model.AdsSkills)
            {
                skill.AdvertisementId = model.Id;
                _adsSkillsRepository.Create(skill);
            }

            foreach (var industry in model.AdsIndustries)
            {
                industry.AdvertisemenId = model.Id;
                _adsIndustryRepository.Create(industry);
            }

            _adsRepository.SaveChanges();
            return entity;

        }

        public IEnumerable<Advertisement> DisplayAds(int count, int companyId = -1)
        {
            var isAuthenticated = HttpContext.Current.Request.IsAuthenticated;
            var isBussinessUser = HttpContext.Current.User.IsInRole(Constants.CompanyRole);
            var company = _companyRepository.Query().Include("CompanySkills")
                                                                    .FirstOrDefault(x => x.Id == companyId);
            //build query
            var query = _adsRepository.Query()
                                      .Include(nameof(Advertisement.AdsIndustries))
                                      .Include(nameof(Advertisement.AdsSkills));

            //settings check
            query = query.Where(x => isAuthenticated ? x.ApearOnLogin : x.ApearOnLogout);
            if (isAuthenticated)
                query = query.Where(x => isBussinessUser ? x.ApearForCompany : x.ApearForPrivatePerson);

            query = query.Where(x => x.StartDate < DateTime.Now && (x.EndDate == null || x.EndDate > DateTime.Now));
            query = query.Where(x => x.Status == AdvertisementStatus.Active);

            //budget check
            query = query.Where(x => x.Budget > x.ClickPrice || x.CreatedByAdmin);

            //order by click price
            query = query.OrderByDescending(x => x.CreatedByAdmin).ThenByDescending(x => x.ClickPrice);

            //only with certain location type
            if (company != null)
            {
                query = query.Where(x => (x.LocationType == LocationType.Cities && x.Location == company.City)
                    || (x.LocationType == LocationType.Country && x.Location == company.Country)
                    || (x.LocationType == LocationType.WithoutLocation));
            }

            var matchedAds = new List<Advertisement>();

            if (isBussinessUser)
            {
                matchedAds = query.ToList(); //.ForCompany(company, count);
            }
            else if (!isBussinessUser)
            {
                //TODO:
                //return query.ToList().ForEmployees();
                matchedAds = query.Take(count).ToList();
            }
            else
            {
                //TODO:
                //return query.ToList().ForUnassigned();
                matchedAds = query.Take(count).ToList();
            }

            return matchedAds;
        }

        public List<Advertisement> GetAllForUser(Guid userId)
        {
            var allAds = GetAllAds();
            return allAds
                        .Where(x => x.UserId == userId.ToString() && x.Status != AdvertisementStatus.Stopped)
                        .ToList();
        }

        public List<Advertisement> GetAll()
        {
            var allAds = GetAllAds();
            return allAds
                .Where(a => a.Status != AdvertisementStatus.Stopped)
                .ToList();
        }

        private IQueryable<Advertisement> GetAllAds()
        {
            return _adsRepository.Query()
                .Include(nameof(Advertisement.AdsSkills))
                .Include(nameof(Advertisement.AdsIndustries))
                .Include("AdsIndustries.Industry")
                .Include("AdsSkills.Skill")
                .Include("User").Include("User.Company");
        }

        public Advertisement GetById(int id)
        {
            return _adsRepository.Query()
                                .Include(nameof(Advertisement.AdsSkills))
                                .Include(nameof(Advertisement.AdsIndustries))
                                .Include("AdsIndustries.Industry")
                                .Include("AdsSkills.Skill")
                                .FirstOrDefault(x => x.Id == id);
        }

        public Advertisement GetById(int id, Guid userId)
        {
            return _adsRepository.Query()
                                 .Include(nameof(Advertisement.AdsSkills))
                                 .Include(nameof(Advertisement.AdsIndustries))
                                 .Include("AdsIndustries.Industry")
                                 .Include("AdsSkills.Skill")
                                 .FirstOrDefault(x => x.Id == id && x.UserId == userId.ToString());
        }

        public Advertisement OpenAd(int id, string currentUserId, string ipAddress)
        {
            var ad = _adsRepository.GetById(id);
            if (ad != null && ad.Status != AdvertisementStatus.Stopped)
            {
                var today = DateTime.Now.Date;
                bool alreadyClicked = _adClicksRepository
                                          .Query()
                                          .FirstOrDefault(c =>
                                                DbFunctions.TruncateTime(c.DateTime) == today
                                                  && c.IpAddress == ipAddress
                                                  && c.AdvertisementId == ad.Id)
                                            != null;
                var isSameUser = !string.Equals(ad.UserId, currentUserId);
                bool isChargable = isSameUser && !alreadyClicked;
                if (isChargable)
                {
                    ad.ClicksCount++;
                    ad.BudgetSpent += ad.ClickPrice;
                    ad.Budget -= ad.ClickPrice;
                }

                bool noMoreBudget = ad.BudgetSpent > ad.Budget || ad.Budget < ad.ClickPrice;
                if (noMoreBudget)
                {
                    ad.Status = AdvertisementStatus.Stopped;
                }

                SaveClickOnAd(currentUserId, ad, isChargable, ipAddress);
            }

            _adsRepository.SaveChanges();
            return ad;
        }

        private void SaveClickOnAd(string currentUserId, Advertisement ad, bool isChargable, string ipAddress)
        {
            var adClick = new AdClick
            {
                DateTime = DateTime.Now,
                AdvertisementId = ad.Id,
                Charged = isChargable ? ad.ClickPrice : 0,
                IpAddress = ipAddress,
                UserId = currentUserId == Guid.Empty.ToString() ? null : currentUserId
            };

            _adClicksRepository.Create(adClick);
            _adClicksRepository.SaveChanges();
        }

        public void Remove(int id, Guid userId)
        {
            var ad = GetById(id, userId);

            if (ad != null)
            {
                var path = HttpContext.Current.Server.MapPath(ad.FilePath);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                _adsRepository.Remove(id);
                _adsRepository.SaveChanges();
            }
        }

        public Advertisement Update(Advertisement model)
        {
            model.FilePath = UploadFile(model.FilePath, model.Type);
            var entity = _adsRepository.Update(model);
            _adsRepository.SaveChanges();

            return entity;
        }

        public string CheckAdsVisibilityForCurrentUser(Advertisement ad)
        {
            var isCompany = HttpContext.Current.User.IsInRole(Constants.CompanyRole);
            var message = new StringBuilder();

            if (!ad.ApearOnLogin)
            {
                message.Append("Ad is not visible for logged in users");
                message.Append(Environment.NewLine);
            }
            else if (!ad.ApearForCompany && isCompany)
            {
                message.Append(Environment.NewLine);
                message.AppendLine("Ad is not visible for companies");
            }
            else if (ad.Status != AdvertisementStatus.Active || (ad.StartDate > DateTime.Now || ad.EndDate < DateTime.Now))
            {
                message.Append(Environment.NewLine);
                message.AppendLine("Ad is inactive.");
            }
            else
            {
                var ads = DisplayAds(12);

                if (!ads.Contains(ad))
                {
                    message.AppendLine("There are ads with better target group match. It can be due click price, geolocation settings, skills or industry code settings.");
                    message.Append(Environment.NewLine);
                }
            }


            if (message.Length == 0)
            {
                message.AppendLine("Ad is visible for current user");
                message.Append(Environment.NewLine);
            }

            return message.ToString();
        }

        private string UploadFile(string base64Path, AdvertisementType type)
        {
            if (base64Path.Contains(type == AdvertisementType.Image ? Constants.AdsImagesPath : Constants.AdsVideoPath))
                return base64Path;

            var path = string.Empty;
            var base64String = base64Path.Split(';')[1].Split(',')[1];
            var extension = base64Path.Split(';')[0].Split(':')[1].Split('/')[1];
            var fileBytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
            {
                var directoryPath = HttpContext.Current.Server.MapPath(type == AdvertisementType.Image ? Constants.AdsImagesPath : Constants.AdsVideoPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                //upload file
                var filePathGuid = $"{Guid.NewGuid().ToString()}.{extension}";
                var fullPath = Path.Combine(directoryPath, filePathGuid);
                FileUploader.UploadFile(ms, fullPath);

                path = type == AdvertisementType.Image
                    ? $"{Constants.AdsImagesPath}{filePathGuid}"
                    : $"{Constants.AdsVideoPath}{filePathGuid}";
            }

            return path;
        }

        public void TransferBudget(int currentId, int transferAdId, decimal amount, bool transferToAnother, out string error)
        {
            error = string.Empty;

            var currentAd = _adsRepository.GetById(currentId);
            var transferAd = _adsRepository.GetById(transferAdId);

            if (transferToAnother)
            {
                error = TransferBudgetToAd(currentAd, transferAd, amount);
            }
            else
            {
                error = TransferBudgetToAd(transferAd, currentAd, amount);
            }

            _adsRepository.SaveChanges();
        }

        private string TransferBudgetToAd(Advertisement currentAd, Advertisement transferAd, decimal amount)
        {
            string error = string.Empty;
            var currentBudget = currentAd.Budget - currentAd.BudgetSpent;
            if (currentBudget < decimal.Zero || (currentBudget - amount) < decimal.Zero)
            {
                error = (string)ResourceString.Instance.NotEnoughBudget.Replace("{ad}", currentAd?.Headline);
            }
            else
            {
                currentAd.Budget -= amount;
                transferAd.Budget += amount;
                AddLogItem($"Budged ({amount} dk) from ad \"{currentAd.Headline}\" with id #{currentAd.Id} was transfered to ad \"{transferAd.Headline}\" with id #{transferAd.Id}.");
            }

            return error;
        }

        public decimal StopAdvertisement(int id)
        {
            decimal remainingBudged = decimal.Zero;
            var ad = _adsRepository.GetById(id);
            if (ad != null)
            {
                ad.Status = AdvertisementStatus.Stopped;
                _adsRepository.SaveChanges();
                remainingBudged = ad.Budget - ad.BudgetSpent;
                AddLogItem($"The advertisement \"{ad.Headline}\" with id #{ad.Id} was stopped.");
            }
            return remainingBudged;
        }
    }
}
