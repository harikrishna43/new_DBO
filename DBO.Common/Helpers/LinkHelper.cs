using System;

namespace DBO.Common.Helpers
{

    public static class LinkHelper
    {
        public static string GetCorrectLink(string url)
        {
            bool isCorrectUrl = Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return isCorrectUrl ? url : $"http://{url}";

        }
    }
}
