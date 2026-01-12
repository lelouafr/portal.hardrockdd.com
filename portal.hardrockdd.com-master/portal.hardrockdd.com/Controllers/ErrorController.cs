using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Web;
using System;
using System.Linq;
using System.Web.Mvc;

namespace portal.Controllers
{
    public class ErrorController : Controller
    {
        [AllowAnonymous]
        [Route("Error/Index/{ErrorId}")]
        [HttpGet]
        public ActionResult Index(int ErrorId)
        {
            using var db = new VPContext();
            var result = db.ErrorLogs.FirstOrDefault(f => f.ErrorId == ErrorId);
            var model = new ErrorLogViewModel(result);
            return View(model);
        }


        [AllowAnonymous]
        [HttpGet]
        public PartialViewResult IndexPartial(int ErrorId)
        {
            using var db = new VPContext();
            var result = db.ErrorLogs.FirstOrDefault(f => f.ErrorId == ErrorId);
            if (ErrorId == 0)
            {
                result = new ErrorLog();
            }
            var model = new ErrorLogViewModel(result);
            HttpContext.Response.Headers.Add("errorId", string.Format("{0}", model.ErrorId));
            return PartialView(model);
        }

        [AllowAnonymous]
        [Route("Error/DNE/{ErrorId}")]
        [HttpGet]
        public ActionResult DoesNotExist(int ErrorId)
        {
            using var db = new VPContext();
            var result = db.ErrorLogs.FirstOrDefault(f => f.ErrorId == ErrorId);
            var model = new ErrorLogViewModel(result);
            return View(model);
        }

        [AllowAnonymous]
        [Route("Error/NotFound/")]
        [HttpGet]
        public ActionResult NotFound()
        {

            if (HttpContext.Request.IsAjaxRequest())
            {
                return PartialView("NotFoundPartial");
            }
            this.Response.StatusCode = 404;
            return View();
        }

        public ActionResult InternalServerError()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("Error/Denied")]
        [HttpGet]
        public ActionResult AccessDenied()
        {
            var curEmp = StaticFunctions.GetCurrentHREmployee();
            if (curEmp == null)
            {
                ViewBag.Layout = "~/Views/Shared/_LayoutEmpty.cshtml";
            }
            else if (curEmp.ActiveYN == "N" || curEmp.PortalAccountActive == "N")
            {
                ViewBag.Layout = "~/Views/Shared/_LayoutEmpty.cshtml";
            }
            this.Response.StatusCode = 403;
            return View();
        }

        [AllowAnonymous]
        [Route("Error/NoDBError")]
        [HttpGet]
        public ActionResult NoDBError()
        {
            return View();
        }

        /// <summary>
        /// Used for unspecified errors
        /// </summary>
        /// <param name="code">
        /// The error code (defaults to 500)
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult" /> encapsulating an unspecified error
        /// </returns>
        public ActionResult BadRequest(int? code)
        {
            this.Response.StatusCode = code ?? 500;

            return this.View();
        }
    }
}