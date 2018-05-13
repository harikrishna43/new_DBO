using DBO.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DBO.Data.Filters
{
    public static class AdsFilter
    {
        public static List<Advertisement> ForCompany(this IEnumerable<Advertisement> ads, Company company, int count)
        {
            var companySkills = company?.CompanySkills?.Select(x => x.SkillId) ?? new List<int>();
            var industryCode = company?.IndustryCode ?? -1;
            var matchedAds = new List<Advertisement>();

            if (ads != null)
            {
                foreach (var ad in ads)
                {
                    if (ad.Location != null && ad.Location.Equals(company?.City, StringComparison.OrdinalIgnoreCase))
                    {
                        matchedAds.Add(ad);
                    }
                    else if (ad.AdsSkills != null && ad.AdsSkills.Any(x => companySkills.Contains(x.SkillId)))
                    {
                        matchedAds.Add(ad);
                    }
                    else if (ad.AdsIndustries != null && ad.AdsIndustries.Any(x => x.IndustryId == industryCode))
                    {
                        matchedAds.Add(ad);
                    }
                }

                if (matchedAds.Count < count)
                {
                    matchedAds.AddRange(ads.Where(x => !matchedAds.Contains(x))
                              .Take(count - matchedAds.Count));
                }
            }

            return matchedAds;
        }
    }
}
