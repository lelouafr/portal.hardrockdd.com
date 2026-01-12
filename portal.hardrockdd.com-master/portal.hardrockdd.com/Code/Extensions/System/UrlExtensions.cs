using System;
using System.Web;
using System.Web.Mvc;

namespace portal
{
    public static class UrlExtensions
    {
        public static string Content(this UrlHelper urlHelper, string contentPath, bool toAbsolute = false)
        {
            if (urlHelper == null)
            {
                throw new System.ArgumentNullException(nameof(urlHelper));
            }
            var path = urlHelper.Content(contentPath);
            var url = new Uri(HttpContext.Current.Request.Url, path);

            return toAbsolute ? url.AbsoluteUri : path;
        }
    }
}