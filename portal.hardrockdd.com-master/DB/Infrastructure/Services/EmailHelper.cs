
using System;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Web.Routing;

namespace DB.Infrastructure.Services
{
    public class EmailHelper
    {
        bool invalid = false;

        private static SmtpClient PortalClient()
        {
            var credentialUserName = "noreply@hardrockdd.com";
            var pwd = "Hardrock2020$!";

            NetworkCredential credentials = new NetworkCredential(credentialUserName, pwd);

            SmtpClient client = new SmtpClient()
            {
                Host = "smtp.office365.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };
            client.Credentials = credentials;
            return client;
        }

        public static Task SendAsync(MailMessage message)
        {
            Task result;
            if (message == null)
            {
                throw new System.ArgumentNullException(nameof(message));
            }
            using var client = PortalClient();
            message.From = (new MailAddress("noreply@hardrockdd.com"));
            try
            {
                if (message.To.Count > 0)
                {
                    result = client.SendMailAsync(message);
                }
                else
                    result = null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.GetBaseException().ToString());
            }

            return result;
        }

        public static void Send(MailMessage message, bool noTest = true)
        {
            if (message == null)
            {
                throw new System.ArgumentNullException(nameof(message));
            }
            using var client = PortalClient();
            message.From = (new MailAddress("noreply@hardrockdd.com"));

            if (HttpContext.Current != null)
            {

                //if ((!HttpContext.Current.Request.Url.AbsoluteUri.ToLower().Contains("test") &&
                //    !HttpContext.Current.Request.Url.AbsoluteUri.ToLower().Contains("localhost")) || noTest == false)
                //{
                    if (message.To.Count > 0)
                    {
                        client.Send(message);
                    }
                //}
                //else
                //{
                //    foreach (var to in message.To.ToList())
                //    {
                //        message.To.Remove(to);
                //    }
                //    foreach (var cc in message.CC.ToList())
                //    {
                //        message.CC.Remove(cc);
                //    }
                //    var curUserEmail = HttpContext.Current.User.Identity.GetUserName();
                //    message.To.Add(new MailAddress(curUserEmail));

                //    message.Subject = string.Format("{0}: {1}", "TEST SYSTEM EMAIL", message.Subject);
                //    client.Send(message);
                //}
            }
            else
            {
                if (message.To.Count > 0)
                {
                    client.Send(message);
                }
            }
        }

        public static string RenderViewToString(string viewPath,
                                    object model = null,
                                    bool partial = false)
        {
            var context = HttpContext.Current;
            var contextBase = new HttpContextWrapper(context);
            //var controllerContext = new HttpContextWrapper(HttpContext.Current);
            var routeData = context.Request.RequestContext.RouteData;
            //routeData.Values.Add("controller", "Home");

            var controllerContext = new ControllerContext(contextBase,
                                                          routeData,
                                                          new EmptyController());

            return RenderViewToString(controllerContext, viewPath, model, partial);

        }

        public static string RenderViewToString(ControllerContext context,
                                    string viewPath,
                                    object model = null,
                                    bool partial = false)
        {
            if (context == null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult;
            if (partial)
                viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            else
                viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, "");

            if (viewEngineResult == null || viewEngineResult.View == null)
                throw new FileNotFoundException("View cannot be found.");

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result = null;

            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view,
                                            context.Controller.ViewData,
                                            context.Controller.TempData,
                                            sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }


      
        public bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format.
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }

        public static List<string> GetEmailList(string GroupName, string ContactTable)
        {
            using var db = new DB.Infrastructure.ViewPointDB.Data.VPContext();

            return (from d in db.PMDistributionGroups
                             join gc in db.PMDistributionGroupContacts on
                                 new { p1 = d.KeyID }
                                 equals
                                 new { p1 = gc.DistributionGroupID }
                             join c in db.PMContacts on
                                 new { p1 = gc.ContactKeyID }
                                 equals
                                 new { p1 = c.KeyID }
                             where
                                d.GroupName == GroupName
                             && gc.ContactTable == ContactTable
                             select c.EMail
                             ).ToList();
        }
    }
    class EmptyController : ControllerBase
    {
        protected override void ExecuteCore() { }
    }
}