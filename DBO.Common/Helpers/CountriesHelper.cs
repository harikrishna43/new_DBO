using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DBO.Common.Helpers
{
    public static class CountriesHelper
    {
        public static List<string> GetCountriesList()
        {
            List<string> countries = new List<string>();
            var infoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo info in infoList)
            {
                RegionInfo region = null;
                try
                {
                    region = new RegionInfo(info.LCID);
                }
                catch (Exception ex)
                {
                    //todo: refactor
                }

                if (region != null)
                {
                    countries.Add(region.EnglishName);
                }
            }

            return countries?.OrderBy(c => c)?.Distinct()?.ToList();
        }
    }
}
