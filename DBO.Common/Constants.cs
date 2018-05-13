using System.IO;

namespace DBO.Common
{
    public static class Constants
    {
        public const string AdminRole = "admin";
        public const string CompanyRole = "company";
        public const string EmployeeRole = "employee";
        public const string EmployeeRoleId = "155588e3-e1b0-4a3c-9489-213cc9ffa4f4";


        public const string AdminEmail = "admin@dbo.as";

        public const int CompaniesPageSize = 10;
        public const int NewsPageSize = 10;

        public const string UserIdClaim = "Id";
        public const string CompanyIdClaim = "CompanyId";

        public const string NewsImagesPath = "~/NewsImages/";
        public const string CompanyLogosPath = "~/img/CompanyLogos/";
        public const string AdsImagesPath = "/img/Ads/";
        public const string AdsVideoPath = "/video/Ads/";
        public const string UserImagePath = "/img/UserFiles/";


        public const string EnglishLanguage = "English";
        public const string DanishLanguage = "Danish";
        public const string DefaultLanguage = "English";

        public const string CookieLanguage = "_language";

        public const string NewsfeedLink = "/home/newsfeed";

        public const string CurrentTimeZoneId = "Central European Standard Time";
    }
}
