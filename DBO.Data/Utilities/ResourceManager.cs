using DBO.Common;
using DBO.Data.Models;
using System;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace DBO.Data.Utilities
{
    public class ResourceString : DynamicObject
    {
        public static dynamic Instance = new ResourceString();
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = string.Empty;
            var resource = Get(binder.Name);

            if (resource != null)
            {
                result = resource;
            }

            return true;
        }


        public static string Get(string name)
        {
            Resource item;
            try
            {
                var languageName = Constants.DefaultLanguage;
                var request = GetRequest();

                if (request != null)
                {
                    languageName = GetRequest().Cookies[Constants.CookieLanguage]?.Value;
                }

                // for some reason this cache has issues in Azure
                //var item = HttpRuntime.Cache.Get(name) as Resource;

                //if (item == null)
                {
                    using (var context = new ApplicationDbContext())
                    {
                        var language = context.Languages.FirstOrDefault(x => x.Name == languageName);
                        if (language == null)
                        {
                            language = context.Languages.FirstOrDefault(x => x.Name == Constants.DefaultLanguage);
                        }

                        item = context.Resources.FirstOrDefault(r => r.Name == name && r.LanguageId == language.Id);
                        //if (item != null)
                        //{
                        //    HttpRuntime.Cache.Insert(name, item);
                        //}
                    }
                }
            }
            catch(Exception exception)
            {
                item = null;
            }

            return item?.Value ?? " "; 
        }

        public static HttpRequest GetRequest()
        {
            HttpRequest request = null;

            try
            {
                request = HttpContext.Current.Request;
            }
            catch(Exception ex)
            {
                request = null;
            }

            return request;
        }

    }
}

