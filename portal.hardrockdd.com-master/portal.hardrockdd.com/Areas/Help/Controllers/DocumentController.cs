using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Help.Controllers
{
    [RouteArea("Help")]
    public class DocumentController : portal.Controllers.BaseController
    {
        #region Applicant List
        [HttpGet]
        [Route("Documents")]
        public ActionResult Index()
        {            
            return View("List/Index");
        }

        [HttpGet]
        public ActionResult Table()
        {
            return PartialView("List/_Table");
        }
        #endregion
    }
}